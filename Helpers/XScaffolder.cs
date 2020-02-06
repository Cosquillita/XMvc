using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Design;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.IO;
using XScaffolding.Models;

namespace XScaffolding.Helpers
{
    public enum TemplateType
    {
        None,
        Controller,
        ApiController,
        View_IndexOrList,
        View_Details,
        View_CreateOrEdit,
        View_Delete
    }

    public class XScaffolder
    {
        public List<string> ScaffoldAllEntitiesFromModel(DbContext context)
        {
            List<string> result = new List<string>();
            var adapter = (IObjectContextAdapter)context;
            var objectContext = adapter.ObjectContext;
            MetadataWorkspace workspace = objectContext.MetadataWorkspace;
            ItemCollection itemCol = workspace.GetItemCollection(DataSpace.OSpace);
            StringBuilder sb = new StringBuilder();
            foreach (EdmType eType in itemCol)
            {
                // filter to only tables and views
                if (eType.GetType().BaseType == typeof(System.Data.Entity.Core.Metadata.Edm.EntityType))
                {
                    // create controller and views for entity
                    ScaffoldEntity(eType);
                }
            }
            return result;
        }

        public void ScaffoldEntity(EdmType eType)
        {
            System.Data.Entity.Core.Metadata.Edm.EntityType entityType = (System.Data.Entity.Core.Metadata.Edm.EntityType)eType;

            // items in eType
            //FullName - XScaffolding.Models.Customers
            //KeyMembers - pk columns
            //KeyProperties - same
            //Members - columns
            //Properties - same
            //MetadataProperties
            //Name - Customers
            //NamespaceName - XScaffolding.Models
            //NavigationProperties - foreign keys

            // add from entities name in web.config connection strings
            // to do : fix this to create for all connection strings that are EF
            string dbContextName = "NorthwindEntities";
            string pkName = entityType.KeyMembers[0].Name;
            string pkDataType = null;
            string modelName = entityType.Name;
            System.Data.Entity.Design.PluralizationServices.PluralizationService ps = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);
            string modelNamePlural = ps.Pluralize(modelName);
            string modelNamePluralLower = modelNamePlural.ToLower();
            string nameSpace = entityType.NamespaceName;
            List<string> asdf = nameSpace.Split('.').ToList();
            string nameSpaceRoot = asdf.First();
            List<string> columnNames = new List<string>();
            foreach (var v in entityType.Members)
            {
                columnNames.Add(v.Name);
            }

            string edmxFileName = @"C:\Users\mwheaton\Documents\Visual Studio 2017\Projects\XScaffolding\XScaffolding\Models\Model1.edmx";
            XElement xfile = XElement.Load(edmxFileName);
            XElement edmxRuntime = xfile.Elements().First();
            XElement edmxConceptualModels = edmxRuntime.Elements().Where(m => m.Name.LocalName == "ConceptualModels").First();
            XElement edmxConceptualSchema = edmxConceptualModels.Elements().First();
            IEnumerable<XElement> items = edmxConceptualSchema.Elements().Where(m => m.Name.LocalName == "EntityType");
            bool found = false;
            foreach (var item in items)
            {
                if (modelName == item.FirstAttribute.Value)
                {
                    pkDataType = "";
                    //pkName = item.Elements().Where(m => m.Name.LocalName == "Key").First().Elements().Where(m => m.Name.LocalName == "PropertyRef").First().FirstAttribute.Value;
                    foreach (XElement xe in item.Elements().Where(m => m.Name.LocalName == "Property")) // skip key and navigation properties
                    {
                        if (xe.FirstAttribute.Value == pkName)
                        {
                            pkDataType = xe.Attribute("Type").Value.StartsWith("Int") ? "int" : "string";
                            found = true;
                            break;
                        }
                    }
                }
                if (found) break;
            }

            string outputPath = System.Web.HttpContext.Current.Server.MapPath("~/Output");
            TextFileUtilities xsfUtils = new TextFileUtilities();
            System.IO.Directory.CreateDirectory(outputPath + @"\Controllers");
            System.IO.Directory.CreateDirectory(outputPath + @"\Views");
            System.IO.Directory.CreateDirectory(outputPath + @"\Views\" + modelNamePlural);

            // write controller
            string controller = LoadTemplateByType(TemplateType.Controller);
            controller = controller.Replace(@"$nameSpace$", nameSpace);
            controller = controller.Replace(@"$nameSpaceRoot$", nameSpaceRoot);
            controller = controller.Replace(@"$dbContextName$", dbContextName);
            controller = controller.Replace(@"$modelName$", modelName);
            controller = controller.Replace(@"$modelVariable$", modelName.ToLower());
            controller = controller.Replace(@"$modelNamePlural$", modelNamePlural);
            controller = controller.Replace(@"$modelNamePluralLower$", modelNamePluralLower);
            controller = controller.Replace(@"$pkName$", pkName);
            xsfUtils.WriteTextFile(outputPath + @"\Controllers\" + modelNamePlural + "Controller.cs", controller);

            string apiController = LoadTemplateByType(TemplateType.ApiController);
            apiController = apiController.Replace(@"$nameSpace$", nameSpace);
            apiController = apiController.Replace(@"$nameSpaceRoot$", nameSpaceRoot);
            apiController = apiController.Replace(@"$dbContextName$", dbContextName);
            apiController = apiController.Replace(@"$modelName$", modelName);
            apiController = apiController.Replace(@"$modelVariable$", modelName.ToLower());
            apiController = apiController.Replace(@"$modelNamePlural$", modelNamePlural);
            apiController = apiController.Replace(@"$modelNamePluralLower$", modelNamePluralLower);
            apiController = apiController.Replace(@"$pkName$", pkName);
            apiController = apiController.Replace(@"$pkDataType$", pkDataType);
            xsfUtils.WriteTextFile(outputPath + @"\Controllers\" + modelNamePlural + "ApiController.cs", apiController);

            // write index
            string index = LoadTemplateByType(TemplateType.View_IndexOrList);
            index = index.Replace(@"$nameSpace$", nameSpace);
            index = index.Replace(@"$nameSpaceRoot$", nameSpaceRoot);
            index = index.Replace(@"$dbContextName$", dbContextName);
            index = index.Replace(@"$modelName$", modelName);
            index = index.Replace(@"$modelNamePlural$", modelNamePlural);
            index = index.Replace(@"$modelNamePluralLower$", modelNamePluralLower);
            index = index.Replace(@"$pkName$", pkName);
            string sortRowItems = null;
            string filterRowItems = null;
            string dataRowItems = null;
            string sortRowItem = @"<th><a href=""#"" onclick=""SortColumn('$modelNamePlural$', '$columnName$', this);""><i class=""fa""></i><span>$columnName$</span></a></th>";
            string filterRowItem = @"<th><input type=""text"" class=""form-control"" onkeydown=""FilterColumn('$modelNamePlural$', '$columnName$', this, event);"" value="""" /></th>";
            string dataRowItem = @"<td>@item.$columnName$</td>";
            int pagerColSpan = 1;
            foreach (var v in columnNames)
            {
                pagerColSpan += 1;
                string s = sortRowItem.Replace(@"$modelName$", modelName);
                s = s.Replace(@"$modelNamePlural$", modelNamePlural);
                s = s.Replace(@"$columnName$", v);
                sortRowItems = sortRowItems + System.Environment.NewLine + s;

                string f = filterRowItem.Replace(@"$modelName$", modelName);
                f = f.Replace(@"$modelNamePlural$", modelNamePlural);
                f = f.Replace(@"$columnName$", v);
                filterRowItems = filterRowItems + System.Environment.NewLine + f;

                string d = dataRowItem.Replace(@"$columnName$", v);
                dataRowItems = dataRowItems + System.Environment.NewLine + d;
            }
            index = index.Replace("{sortRowItems}", sortRowItems);
            index = index.Replace("{filterRowItems}", filterRowItems);
            index = index.Replace("{dataRowItems}", dataRowItems);
            index = index.Replace(@"$pagerColSpan$", pagerColSpan.ToString());

            string edit = LoadTemplateByType(TemplateType.View_CreateOrEdit);
            edit = edit.Replace("$modelName$", modelName);
            edit = edit.Replace("$pkName$", pkName);
            string columns = null;
            foreach (var v in columnNames)
            {
                columns = columns + EditTemplate(v);
            }
            edit = edit.Replace("{columns}", columns);

            xsfUtils.WriteTextFile(outputPath + @"\Views\" + modelNamePlural + @"\Edit.cshtml", edit);
            xsfUtils.WriteTextFile(outputPath + @"\Views\" + modelNamePlural + @"\Index.cshtml", index);
        }

        public string EditTemplate(string columnName)
        {
            string template = @"
            <div class=""form-group"">
                @Html.LabelFor(model => model.$columnName$, htmlAttributes: new { @class = ""control-label col-md-2"" })
                <div class=""col-md-10"">
                    @Html.EditorFor(model => model.$columnName$, new { htmlAttributes = new { @class = ""form-control"" } })
                    @Html.ValidationMessageFor(model => model.$columnName$, """", new { @class = ""text-danger"" })
                </div>
            </div>
            ";
            return template.Replace("$columnName$", columnName);
        }

        public string LoadTemplateByType(TemplateType templateType)
        {
            string result = null;
            string templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Scaffolders");

            switch (templateType)
            {
                case TemplateType.ApiController:
                    templatePath = templatePath + @"/Controllers/ApiController.cs.pp";
                    break;
                case TemplateType.Controller:
                    templatePath = templatePath + @"/Controllers/Controller.cs.pp";
                    break;
                case TemplateType.View_IndexOrList:
                    templatePath = templatePath + @"/Views/Index.cshtml.pp";
                    break;
                case TemplateType.View_Details:
                    templatePath = templatePath + @"/Views/_Details.cshtml";
                    break;
                case TemplateType.View_CreateOrEdit:
                    templatePath = templatePath + @"/Views/Edit.cshtml.pp";
                    break;
                case TemplateType.View_Delete:
                    templatePath = templatePath + @"/Views/_Delete.cshtml";
                    break;
            }
            if (templatePath == System.Web.HttpContext.Current.Server.MapPath("~/Scaffolders")) return null;

            TextFileUtilities xsfUtils = new TextFileUtilities();
            result = xsfUtils.ReadTextFile(templatePath);
            return result;
        }
    }
}
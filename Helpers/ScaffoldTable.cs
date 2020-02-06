using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XScaffolding.Controllers
{
    public class ScaffoldTable
    {
        public string CreateTableHtmlElement()
        {
            List<Document> docs = new List<Document>();
            string table =
    @"
    <table class=""table"">
        <thead><tr>
            <th><a href=""/Documents/Create"">Create New</a></th>
            <th>GeneratedFileName</th>
            <th>UserFileName</th>
            <th>Description</th>
            <th>Size</th>
            <th>Created</th>
            <th>CreatedBy</th>
            <th>DocumentTypeId</th>
            <th>TrainingCourseId</th>
		    </tr>
	    </thead>
	    <tbody>
		    {records}
	    </tbody>
    </table>
";
            string records = null;
            foreach (Document d in docs)
            {
                records += CreateHtmlTableRow(d);
            }
            return table.Replace("{records}", records);
        }

        public string CreateHtmlTableRow(Document d)
        {
            string Id = d.Id.ToString();
            string GeneratedFileName = d.GeneratedFileName;
            string UserFileName = d.UserFileName;
            string Size = d.Size.ToString();
            string Description = d.Description;
            string Created = d.Created.ToShortDateString();
            string CreatedBy = d.CreatedBy;
            string DocumentTypeId = d.DocumentTypeId.ToString();
            string TrainingCourseId = d.TrainingCourseId.ToString();
            string row =
$@"
	<tr>
    <td>
        <a href=""/Documents/Edit/{Id}"">Edit</a> |
        <a href=""/Documents/Details/{Id}""> Details </a> |
        <a href=""/Documents/Delete/{Id}"">Delete</a>
    </td>
    <td>{GeneratedFileName}</td>
    <td>{UserFileName}</td>
    <td>{Description}</td>
    <td>{Size}</td>
    <td>{Created}</td>
    <td>{CreatedBy}</td>
    <td>{DocumentTypeId}</td>
    <td>{TrainingCourseId}</td>
	</tr>
";
            return row;
        }
    }

    public class Document
    {
        public Document()
        {
        }
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> TrainingCourseId { get; set; }
        public string GeneratedFileName { get; set; }
        public string UserFileName { get; set; }
        public string Description { get; set; }
        public Nullable<int> Size { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }
}
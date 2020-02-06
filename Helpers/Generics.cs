using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Reflection;
using System.Text;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using XScaffolding.Models;

namespace XScaffolding.Helpers
{
    public static class Generics
    {
        public const char ampersandd = '╬';
        public const char equalSignn = '═';
        public static string GetTableNameFromExpression<TModel, TValue>(Expression<Func<TModel, TValue>> expression, bool pluralize = true)
        {
            if (pluralize)
            {
                PluralizationService pluralizer = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);
                return pluralizer.Pluralize(((MemberExpression)expression.Body).Expression.Type.Name);
            }
            else
            {
                return ((MemberExpression)expression.Body).Expression.Type.Name;
            }
        }

        public static string GetColumnNameFromExpression<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        private static string GetDisplayNameFromPropertyAttribute<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            string result = null;
            string propertyName = GetColumnNameFromExpression(expression);
            bool success = true;
            try
            {
                var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => Activator.CreateInstance<TModel>(), typeof(TModel), propertyName);
                result = (metadata.DisplayName ?? propertyName);
                result = result.Replace('_', ' ');
            }
            catch
            {
                success = false;
            }
            if (!success)
            {
                result = TitleCase(propertyName);
            }
            return result;
        }

        private static string GetDisplayNameFromPropertyAttribute<TModel>(string propertyName)
        {
            string result = null;
            bool success = true;
            try
            {
                var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => Activator.CreateInstance<TModel>(), typeof(TModel), propertyName);
                result = (metadata.DisplayName ?? propertyName);
            }
            catch
            {
                success = false;
            }
            if (!success)
            {
                result = TitleCase(propertyName);
            }
            return result;
        }

        public static string TitleCase(string aString)
        {
            string result = "";

            if (aString.Contains("_") || aString.Contains(" "))
            {
                // split string on space or underline and then title case
                // ex : "LAST_NAME" > "Last Name" or "LAST NAME" > "Last Name"
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                result = myTI.ToTitleCase(aString.Trim().Replace("_", " "));
            }
            else
            {
                if (aString.Substring(0, 1) == aString.Substring(0, 1).ToUpper() &&
                    aString.Substring(0, 1) == aString.Substring(0, 1).ToLower())
                {
                    //first char is uppercase and second is not... this may be camel-cased string
                    // ex : "LastName" > "Last Name"
                    result = SplitCamelCase(aString);
                }
                else
                {
                    // just title case the string
                    // ex : "lastname" > "Lastname"
                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                    result = myTI.ToTitleCase(aString);
                }
            }
            return result;
        }

        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static Type GetModelType<T>(IQueryable<T> source)
        {
            return typeof(T);
        }

        public static string GetModelName<T>(IQueryable<T> source)
        {
            return typeof(T).Name;
        }

        public static string GetModelName<T>(T source)
        {
            return typeof(T).Name;
        }

        public static Type GetColumnDataType<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return ((MemberExpression)expression.Body).Member.GetType();
        }

        public static Type GetColumnDataType<T>(IQueryable<T> source, string columnName)
        {
            Type result = null;
            ParameterExpression param = Expression.Parameter(typeof(T), columnName);
            MemberExpression member = Expression.Property(param, columnName);
            result = member.Type;
            return result;
        }

        public static Type GetColumnDataType<T>(T tColumn)
        {
            return tColumn.GetType();
        }

        public static string GetColumnName<T>(T tColumn)
        {
            return tColumn.GetType().Name;
        }

        public static MvcHtmlString FilterFor<TModel, TValue>(
            this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> html,
            Expression<Func<TModel, TValue>> expression,
            SelectList lstSelectList, string defaultValue, Dictionary<string, string> dFilters)
        {
            if (dFilters == null) return new MvcHtmlString("");

            // to do : optimize
            // use generics to get model and column name
            string tableName = GetTableNameFromExpression(expression);
            string columnName = GetColumnNameFromExpression(expression);
            string displayName = GetDisplayNameFromPropertyAttribute(expression);
            Type type = expression.ReturnType;

            string tag = "";

            if (lstSelectList != null)
            {
                // dropdownlist
                // build main tag
                tag =
                    @"
                    <select id=""{columnName}_filter"" name=""{columnName}_filter"" class=""form-control input-xs"" onchange=""FilterTable('{tableName}', '{columnName}', this, event);"">
                        {options}
                    </select>
                    ";
                // tag = tag.Replace("{tableName}", tableName).Replace("{columnName}", columnName);

                // get selected value, if any
                string selectedValue = dFilters.Keys.Contains(columnName) ? selectedValue = dFilters[columnName] : null;

                // add items
                string optionTag = "          " + @"<option value=""{defaultValue}"" {selected}>{defaultValue}</option>";
                StringBuilder sb = new StringBuilder();
                if (defaultValue != null)
                {
                    string selected = defaultValue == selectedValue ? "selected" : "";
                    sb.AppendLine(optionTag);
                }
                foreach (SelectListItem L in lstSelectList)
                {
                    sb.AppendLine(optionTag);
                }

                // add options to tag
                string options = tag.Replace("          " + @"{options}", sb.ToString());
                tag = tag.Replace("          " + @"{options}", sb.ToString());
            }
            else if (type == typeof(string))
            {
                // textbox
                string value = dFilters.Keys.Contains(columnName) ? "" : "";
                tag = @"<input type=""text"" id=""{columnName}_filter"" name=""{columnName}_filter"" value=""{value}"" class=""form-control input-xs"" onkeydown=""FilterTable('{tableName}', '{columnName}', this, event);"" />";
            }
            else if (type == typeof(DateTime))
            {
                // calendar
                string value = dFilters[columnName];
                tag = @"<input type=""text"" id=""{columnName}_filter"" name=""{columnName}_filter"" value=""{value}"" class=""form-control input-xs datepicker"" onchange=""FilterTable('{tableName}', '{columnName}', this, event);"" />";
            }
            else
            {
                // textbox
                string value = dFilters[columnName];
                tag = @"<input type=""text"" id=""{columnName}_filter"" name=""{columnName}_filter"" value=""{value}"" class=""form-control input-xs"" onkeydown=""FilterTable('{tableName}', '{columnName}', this, event);"" />";
            }
            return new MvcHtmlString(tag);
        }

        public static MvcHtmlString SortLinkFor<TModel, TValue>(
            this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> html,
            Expression<Func<TModel, TValue>> expression,
            Dictionary<string, string> dSorts)
        {
            // use generics to get model and column name
            string m = html.GetType().Name;
            string tableName = GetTableNameFromExpression(expression);
            string columnName = GetColumnNameFromExpression(expression);
            string displayName = GetDisplayNameFromPropertyAttribute(expression);
            Type type = expression.ReturnType;

            string tag = $@"<a href=""#"" id=""{columnName}_sort"" name=""{columnName}_sort"" onclick=""SortTable('{tableName}', '{columnName}', this);"" class=""sortUNSORTED"">{displayName}</a>";
            //tag = tag.Replace("{tableName}", tableName).Replace("{columnName}", columnName).Replace("{displayName}", displayName);

            if (dSorts != null && dSorts.Keys.Contains(columnName))
            {
                // if column has a sort, apply the up/down arrow
                string UNSORTED = tag.Replace($"UNSORTED", dSorts[columnName]);
                tag = tag + @"<span class=""sortSprite " + dSorts[columnName] + @"""></span>";
            }
            return new MvcHtmlString(tag);
        }

        public static MvcHtmlString SortLink(
            string tableName,
            string columnName,
            string displayName,
            Dictionary<string, string> dSorts)
        {
            string tag = $@"<a href=""#"" id=""{tableName}_{columnName}_sort"" name=""{tableName}_{columnName}_sort"" onclick=""SortTable('{tableName}', '{columnName}', this);"" class=""sortUNSORTED"">{displayName}</a>";
            if (dSorts != null && dSorts.Keys.Contains(columnName))
            {
                // if column has a sort, 
                // set the sort class
                // and apply the up/down arrow
                tag = tag.Replace($"sortUNSORTED", "sort" + dSorts[columnName]);
                tag = tag + @"<span class=""sortSprite " + dSorts[columnName] + @"""></span>";
            }
            return new MvcHtmlString(tag);
        }

        public static MvcHtmlString FormatDateFromDateTime(DateTime dt)
        {
            return new MvcHtmlString(dt.ToString("MM/dd/yyyy"));
        }

        public static MvcHtmlString FormatDateFromDateTime(DateTime? dt)
        {
            return dt == (DateTime?)null ? new MvcHtmlString("") : new MvcHtmlString(((DateTime)dt).ToString("MM/dd/yyyy"));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> html, Expression<Func<TModel, TValue>> expression)
        {
            string columnCaption = GetDisplayNameFromPropertyAttribute(expression);
            return new MvcHtmlString(string.Format("<label>" + columnCaption + "</label>"));
        }

        public static string DisplayNameFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return GetDisplayNameFromPropertyAttribute(expression);
        }

        public static Dictionary<string, string> ParseUrlStringToDictionary(string urlString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(urlString)) return result;
            System.Collections.Specialized.NameValueCollection NV = System.Web.HttpUtility.ParseQueryString(urlString);
            foreach (string key in NV.Keys)
            {
                result.Add(key, NV[key]);
            }
            return result;
        }

        public static Dictionary<string, string> ParseSpecialUrlStringToDictionary_SingleColumn(string urlString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(urlString)) return result;
            urlString = urlString.DecodeSpecialUrlPart();
            System.Collections.Specialized.NameValueCollection NV = System.Web.HttpUtility.ParseQueryString(urlString);
            foreach (string key in NV.Keys)
            {
                result.Add(key, NV[key]);
            }
            return result;
        }

        /// <summary>
        ///	    just before going to the server, the url needs to be copasetic by switching out excess ampersands with carets and equals with tildes
        ///	    example
        ///	    filters : LAST_NAME=smith&HIRE_TYPE=federal
        ///	    sorts : LAST_NAME=ASCENDING
        ///	    combined url string is this...
        ///	    filters=LAST_NAME=smith&HIRE_TYPE=federal&sorts=LAST_NAME=ASCENDING
        ///	    the above will not parse properly so the url encoding for each item needs to be changed to get this...
        ///	    filters=LAST_NAME~smith^HIRE_TYPE~federal&sorts=LAST_NAME~ASCENDING
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeSpecialUrlPart(this string value)
        {
            // reverse of EncodeUrlPartSpecial
            string result = value == null ? "" : value;
            result = result.Replace('^', '&').Replace('~', '=');
            return result;
        }

        public static string GetDisplayTextFromSelectListValue(string id, SelectList selectList)
        {
            string result = null;
            if (string.IsNullOrWhiteSpace(id))
            {
                return result;
            }
            if (selectList.Where(m => m.Value == id).Count() == 1)
            {
                result = selectList.Where(m => m.Value == id).First().Text;
            }
            return result;
        }



    }
}
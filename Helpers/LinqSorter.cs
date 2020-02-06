using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace XScaffolding.Helpers
{
    public static class LinqSorter
    {
        private static MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);

        public static string asc = "asc";
        public static string desc = "desc";
        public static string ascending = "asc";
        public static string descending = "desc";
        public static string sort_ascending = "asc";
        public static string sort_descending = "desc";
        public static string ASCENDING = "ASCENDING";
        public static string DESCENDING = "DESCENDING";

        public static IQueryable<T> ApplySorts<T>(IQueryable<T> source, Dictionary<string, string> dSorts)
        {
            // 'source.Expression' threw an exception of type 'System.Data.MetadataException'
            // this occurs when the edmx file is not refreshed after adding XScaffolding POCO classes.
            // now using the Model1.tt file to generate these so problem should be solved.
            IQueryable<T> result = source;
            if (dSorts.Keys.Count == 0) return result;
            try
            {
                Expression exp = source.Expression;
                bool isVirgin = true;
                foreach (string key in dSorts.Keys)
                {
                    string columnName = key;
                    bool isAscending = (dSorts[key] == sort_ascending || dSorts[key] == asc) ? true : false;
                    Type type = Generics.GetColumnDataType(source, key);
                    bool isString = type == typeof(string) ? true : false;
                    exp = ApplySingleSort<T>(source, exp, columnName, isAscending, isString, isVirgin);
                    isVirgin = false;
                }
                if (exp != null) result = result.Provider.CreateQuery<T>(exp);
            }
            catch 
            {
            }
            return result;
        }

        public static Expression ApplySingleSort<T>(IQueryable<T> source, Expression exp, string columnName, bool isAscending, bool isString, bool isVirgin)
        {
            Expression result = exp;
            ParameterExpression parameter = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression member = Expression.Property(parameter, columnName);
            LambdaExpression lambda = Expression.Lambda(member, parameter);

            string sortMethodName = isAscending ? "OrderBy" : "OrderByDescending";
            if (!isVirgin) sortMethodName = isAscending ? "ThenBy" : "ThenByDescending";

            Type t = member.Type;
            if (t == typeof(string))
            {
                // if string, add ToLower() to lambda expression to make case insensitive
                Expression columnToLower = Expression.Call(member, toLowerMethod);
                lambda = Expression.Lambda(columnToLower, parameter);
            }

            // add to the sort expression
            result = Expression.Call(
                typeof(Queryable),
                sortMethodName,
                new Type[] { source.ElementType, member.Type },
                result,
                Expression.Quote(lambda));

            return result;
        }
    }

    public class SortObject
    {
        public string columnName = "";
        public string sortDirection = "";
        public string sortOrder = "";
    }
}
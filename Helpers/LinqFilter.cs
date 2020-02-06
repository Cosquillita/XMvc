using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XScaffolding.Helpers
{
    // default text search option is containsMethod
    public static class LinqFilter
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);

        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> source, Dictionary<string, string> dFilters)
        {
            IQueryable<T> result = source;
            if (dFilters.Keys.Count == 0) return result;
            foreach (string key in dFilters.Keys)
            {
                result = ApplySingleFilter<T>(result, key, dFilters[key]);
            }
            return result;
        }

        public static IQueryable<T> ApplySingleFilter<T>(IQueryable<T> source, string columnName, object columnValue)
        {
            IQueryable<T> result = source;
            ParameterExpression param = Expression.Parameter(typeof(T), columnName);
            MemberExpression member = Expression.Property(param, columnName);
            Type t = member.Type;

            if (t.ToString().Contains("System.DateTime") && IsNullableType(t)) // nullable handling for date time
            {
                string s = columnValue.ToString();
                DateTime d;
                if (DateTime.TryParse(s, out d))
                {
                    Nullable<System.DateTime> d2;
                    d2 = (Nullable<System.DateTime>)d;
                    columnValue = d2;
                }
            }
            else
                columnValue = Convert.ChangeType(columnValue, t);

            ConstantExpression constant = Expression.Constant(columnValue);
            Expression exp = null;

            // assume data type is string
            FilterOperator filterOperator = FilterOperator.Contains;
            if (t == typeof(int))
                filterOperator = FilterOperator.Equals;
            if (t == typeof(DateTime))
                filterOperator = FilterOperator.Equals;

            // add the filter type logic
            if (filterOperator == FilterOperator.Contains)
            {
                Expression left = Expression.Call(member, toLowerMethod);
                Expression right = Expression.Constant(columnValue.ToString().ToLower());
                exp = Expression.Call(left, containsMethod, right);
            }
            else if (filterOperator == FilterOperator.Equals)
            {
                if (IsNullableType(t)) // more nullable handling
                    exp = MyEquals(member, constant);
                else
                    exp = Expression.Equal(member, constant);
            }

            // create the delegate and add to the where clause of the IQueryable
            Expression<Func<T, bool>> deleg = Expression.Lambda<Func<T, bool>>(exp, param);            
            result = result.Where(deleg);

            return result;
        }

        #region Nullable Handling

        /// <summary>
        /// nullable test
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// strip nullable type and create a greater than expression
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        private static Expression MyGreaterThan(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
                e2 = Expression.Convert(e2, e1.Type);
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.GreaterThan(e1, e2);
        }

        /// <summary>
        /// strip nullable type and create an equals expression
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        private static Expression MyEquals(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
                e2 = Expression.Convert(e2, e1.Type);
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.Equal(e1, e2);
        }

        #endregion

        private static FilterOperator GetFilterOperatorForJQueryProp(string jQueryProp)
        {
            FilterOperator result = FilterOperator.Equals;
            Dictionary<string, FilterOperator> dictJQuery = new Dictionary<string, FilterOperator>();
            dictJQuery.Add("text", FilterOperator.StartsWith);
            dictJQuery.Add("select-one", FilterOperator.Equals);
            dictJQuery.Add("radio", FilterOperator.Equals);
            dictJQuery.Add("checkbox", FilterOperator.Equals);
            dictJQuery.Add("textarea", FilterOperator.StartsWith);
            if (dictJQuery.Keys.Contains(jQueryProp)) result = dictJQuery[jQueryProp];
            return result;
        }
    }

    public class FilterObject
    {
        public static string columnName = "";
        public static string filter = "";
        public static string filterType = "";
    }

    public enum FilterOperator
    {
        Equals,
        NotEqual,
        Contains,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqualTo,
        StartsWith,
        EndsWith
    }

}


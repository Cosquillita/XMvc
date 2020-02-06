using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Text;

namespace XScaffolding.Helpers
{
    // default text search option is containsMethod
    public static class LinqPager
    {
        public static MvcHtmlString CreatePager(
            string tableName,
            string viewName,
            int pageNum,
            int pageSize,
            int rowCount,
            int pageCount
            )
        {
            string ellipsis = @"...";

            string startUL = @"<ul class=""pagination"">";
            string summary = $"<li><a href=\"#\">Page {pageNum.ToString()} of {pageCount.ToString()} : Records {rowCount.ToString()}</a></li>";

            string firstPage = $@"<li><a href=""#"" onclick=""PagerClick('{tableName}', 1, {pageSize.ToString()});"">First</a></li>";
            string prevPage = $@"<li><a href=""#"" onclick=""PagerClick('{tableName}', {(pageNum-1).ToString()}, {pageSize.ToString()});"">Prev</a></li>";
            string firstPageDisabled = @"<li class=""disabled""><a href=""#"">First</a></li>";
            string prevPageDisabled = @"<li class=""disabled""><a href=""#"">Prev</a></li>";

            string pageLink = $@"<li><a href=""#"" onclick=""PagerClick('{tableName}', <<pageNum>>, {pageSize.ToString()});""><<pageNum>></a></li>";
            string pageEllipsis = @"<li class=""disabled""><a href=""#"">...</a></li>";
            string pageCurrent = $@"<li class=""active""><a href=""#"" style=""text-decoration: none;"">{pageNum.ToString()}</a></li>";

            string nextPage = $@"<li class=""next""><a href=""#"" onclick=""PagerClick('{tableName}', {(pageNum + 1).ToString()}, {pageSize.ToString()});"">Next</a></li>";
            string lastPage = $@"<li class=""last""><a href=""#"" onclick=""PagerClick('{tableName}', {pageCount.ToString()}, {pageSize.ToString()});"">Last</a></li>";
            string nextPageDisabled = @"<li class=""next disabled""><a href=""#"">Next</a></li>";
            string lastPageDisabled = @"<li class=""last disabled""><a href=""#"">Last</a></li>";

            List<string> pages = CreatePagerArray(pageNum, pageCount).ToList();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(startUL);
            sb.AppendLine(string.Format(summary, pageNum, pageCount.ToString(), rowCount.ToString()));

            sb.AppendLine(pageNum == 1 ? prevPageDisabled : prevPage);
            sb.AppendLine(pageNum == 1 ? firstPageDisabled : firstPage);

            int pp = 1;
            foreach (string p in pages)
            {
                if (p == ellipsis)
                    sb.AppendLine(pageEllipsis);
                else if (pageNum == int.Parse(p))
                    sb.AppendLine(pageCurrent);
                else
                    sb.AppendLine(pageLink.Replace(@"<<pageNum>>", pp.ToString()));
                pp++;
            }

            sb.AppendLine(pageNum == pageCount ? nextPageDisabled : nextPage);
            sb.AppendLine(pageNum == pageCount ? lastPageDisabled : lastPage);

            sb.AppendLine(@"</ul>");
            return new MvcHtmlString(sb.ToString());
        }

        public static IQueryable<T> GetPageByPageNumber<T>(IQueryable<T> source, int pageNum, int pageSize, int rowCount)
        {
            // get one page of data by page number
            IQueryable<T> result = source;
            int currentPageNumber = pageNum > 0 ? pageNum : 1;
            if (pageSize > rowCount) result = result.Take(rowCount);
            else
            {
                if (currentPageNumber == 1)
                    result = result.Take(pageSize);
                else
                    // "The method 'Skip' is only supported for sorted input in LINQ to Entities. The method 'OrderBy' must be called before the method 'Skip'."
                    result = result.Skip((currentPageNumber - 1) * pageSize).Take(pageSize);
            }
            return result;
        }

        public static int GetPageCount(int rowCount, int rowsPerPage)
        {
            if (rowCount == 0) return 0;
            int result = 1;
            if (rowCount > 0 && rowsPerPage > 0)
            {
                decimal d = rowCount / rowsPerPage;
                d = d == 0 ? 1 : d;
                result = int.Parse(Math.Floor(d).ToString());
                if (rowCount > rowsPerPage)
                    if (rowCount % rowsPerPage > 0)
                        result += 1;
            }
            return result;
        }

        private static string[] CreatePagerArray(int currentPageNumber, int pageCount)
        {
            string[] result = null;
            if (pageCount <= 12)
            {
                result = new string[pageCount];
                for (int i = 0; i < pageCount; i++)
                    result[i] = (i + 1).ToString();
            }
            else if (currentPageNumber <= 5)
            {
                result = new string[12];
                result[0] = "1";
                result[1] = "2";
                result[2] = "3";
                result[3] = "4";
                result[4] = "5";
                result[5] = "6";
                result[6] = "7";
                result[7] = "...";
                result[8] = (pageCount - 3).ToString();
                result[9] = (pageCount - 2).ToString();
                result[10] = (pageCount - 1).ToString();
                result[11] = (pageCount).ToString();
            }
            else if (currentPageNumber >= (pageCount - 5))
            {
                result = new string[12];
                result[0] = "1";
                result[1] = "2";
                result[2] = "3";
                result[3] = "...";
                result[4] = (pageCount - 7).ToString();
                result[5] = (pageCount - 6).ToString();
                result[6] = (pageCount - 5).ToString();
                result[7] = (pageCount - 4).ToString();
                result[8] = (pageCount - 3).ToString();
                result[9] = (pageCount - 2).ToString();
                result[10] = (pageCount - 1).ToString();
                result[11] = (pageCount).ToString();
            }
            else
            {
                // 1,2,3 (n-1,n,n+1,n+2) x,y,z
                result = new string[12];
                result[0] = "1";
                result[1] = "2";
                result[2] = "3";
                result[3] = "...";
                result[4] = (currentPageNumber - 1).ToString();
                result[5] = (currentPageNumber).ToString();
                result[6] = (currentPageNumber + 1).ToString();
                result[7] = (currentPageNumber + 2).ToString();
                result[8] = "...";
                result[9] = (pageCount - 2).ToString();
                result[10] = (pageCount - 1).ToString();
                result[11] = (pageCount).ToString();
            }
            return result;
        }
    }
}


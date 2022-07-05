using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace VirtualGarage.HtmlHelpers
{
    /// <summary>
    /// Класс, предоставляющий средства для реализации пейджинга
    /// </summary>
    public static class PagingHelper
    {
        /// <summary>
        /// Возвращает разметку для списка страниц с гостиницами
        /// </summary>
        /// <param name="html"></param>
        /// <param name="currentPage">Текущая запрошенная страница</param>
        /// <param name="totalPages">Общее количество страниц</param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public static String PageLinks(this HtmlHelper html, Int32 currentPage,
                                       Int32 totalPages, Func<Int32, String> pageUrl)
        {
            if (totalPages < 2)
            {
                return String.Empty;
            }

            StringBuilder result = new StringBuilder();
            result.Append("Страницы: ");
            for (Int32 i = 1; i <= totalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");

                if (i == currentPage)
                {
                    tag.AddCssClass("selected");
                }
                else
                {
                    tag.MergeAttribute("href", pageUrl(i));
                    tag.MergeAttribute("class", "page");
                }

                tag.InnerHtml = i.ToString();
                result.AppendLine(tag.ToString());
            }

            return result.ToString();
        }
    }
}
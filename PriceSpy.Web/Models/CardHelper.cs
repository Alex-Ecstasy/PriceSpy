using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PriceSpy.Web.Models
{
    public static class CardHelper
    {
        public static HtmlString CreateCard(this IHtmlHelper html)
        {
            string result = "<div class=\"card\" style=\"width: 280px;\">\r\n" +
                            "<img src=\"...\" class=\"rounded mx-auto d-block\" width=\"256\" height=\"200\">\r\n" +
                            "<div class=\"card-body\">\r\n" +
                            "<h5 class=\"card-title\">Описание товара</h5>\r\n" +
                            "<p class=\"card-text\">Нужно ли доп информация?</p>\r\n" +
                            "<a href=\"#\" class=\"btn btn-primary\">Ссылка на товар?</a>\r\n" +
                            "</div>\r\n" +
                            "</div>";

            return new HtmlString(result);
        }
    }
}

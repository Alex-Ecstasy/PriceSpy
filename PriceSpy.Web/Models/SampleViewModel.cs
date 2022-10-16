namespace PriceSpy.Web.Models
{
    public class CardTemplate
    {
        public string? Name { get; set; }
        public string? Price { get; set; } = "0";
        public string? PictureUrl { get; set; }
        public string? CatNumber { get; set; }
        public string? Status { get; set; }
    }

    public class SiteModel
    {
        public string? Name { get; set; }
       
        public ICollection<CardTemplate> CardTemplates { get; set; } = new List<CardTemplate>();

        public int ResultCount
        {
            get
            {
                return CardTemplates.Count;
            }
        }
    }

    public class SampleViewModel
    {
        public ICollection<SiteModel> Sites { get; set; } = new List<SiteModel>();
    }
}

//https://learn.microsoft.com/ru-ru/aspnet/core/mvc/views/display-templates?view=aspnetcore-6.0
using System;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageCreator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var url = @"https://unsplash.com/s/photos/fireworks";
            var splashSite = @"https://unsplash.com";
            
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            //var items = doc.DocumentNode.SelectNodes("//figure[@itemprop='image']").ToList();
            var items = doc.DocumentNode.SelectNodes("//a[@itemprop='contentUrl']")
                .Where(x => x.Attributes["href"].Value.StartsWith("/photos"))
                .ToList();

            foreach (var item in items)
            {
                string photoUrl = $"{splashSite}{item.Attributes["href"].Value}";
                
                HtmlDocument photoPage = web.Load(photoUrl);

                var images = photoPage.DocumentNode.SelectNodes("//img").ToList();

                if (images.Count > 1)
                {
                   Console.WriteLine(images[2].Attributes["src"].Value);   
                }
            }
        }
    }
}
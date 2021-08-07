using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageCreator.Services
{
    /// <summary>
    /// Utilizes https://unsplash.com website (free-use)
    /// </summary>
    public class UnsplashImageService : IImageService
    {
        private static readonly Random Random = new();
        private readonly Dictionary<string, CachedResult> _cache = new();
        
        public string BaseUrl => "https://unsplash.com";
        
        async Task<List<HtmlNode>> Query(string query)
        {
            if (_cache.ContainsKey(query))
            {
                CachedResult result = _cache[query];
                
                // Ensure we use up-to-date images from cache
                if (result.DeleteAfter >= DateTime.Now)
                    return result.Nodes;

                _cache.Remove(query);
            }
            
            string searchUrl = string.Concat("https://unsplash.com/s/photos/", query);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(searchUrl);
            
            // Grab all the links with the photo url
            var images = doc.DocumentNode.SelectNodes("//a[@itemprop='contentUrl']")
                .Where(x => x.Attributes["href"].Value.StartsWith("/photos"))
                .ToList();

            // Cache for later use
            _cache.Add(query, new CachedResult()
            {
                Nodes = images,
                DeleteAfter = DateTime.Now.AddDays(1)
            });
            
            return images;
        }

        public async Task<string> RandomImageUrl(string query)
        {
            var images = await Query(query);

            int index = Random.Next(0, images.Count);
            string photoUrl = $"{BaseUrl}{images[index].Attributes["href"].Value}";
            
            HtmlWeb web = new();
            HtmlDocument photoPage = await web.LoadFromWebAsync(photoUrl);

            var items = photoPage.DocumentNode.SelectNodes("//img").ToList();

            if (items.Count > 1)
                return items[2].Attributes["src"].Value;

            return null;
        }
    }
}
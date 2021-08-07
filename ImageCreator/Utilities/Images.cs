using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageCreator.Utilities
{
    public class Images
    {
        private readonly string _backupImageUrl = @"https://images.unsplash.com/photo-1579403124614-197f69d8187b?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80";
        
        public async Task<string> CreateImageAsync(NewMemberBannerOptions options, string imageUrl = null)
        {
            //user.GetAvatarUrl(ImageFormat.Png, 2048) ?? user.DefaultAvatarUrl
            var avatar = await FetchImageAsync(options.AvatarUrl);
            
            var background = await FetchImageAsync(imageUrl ?? _backupImageUrl);
            background = CropToBanner(background);
            
            // Clip avatar to circle
            avatar = ClipImageToCircle(avatar);

            var bitmap = avatar as Bitmap;
            bitmap?.MakeTransparent();

            var banner = CopyRegionIntoImage(bitmap, background);
            banner = DrawTextToImage(banner, options.Username, options.Subheader);

            string path = $"{Guid.NewGuid()}.png";
            banner.Save(path);
            
            banner.Dispose();
            avatar.Dispose();
            background.Dispose();
            bitmap?.Dispose();
            
            return await Task.FromResult(path);
        }

        private static Random Random = new Random();
        
        /// <summary>
        /// Return a random image from search result
        /// </summary>
        /// <param name="searchText">Category to search unsplash.com for</param>
        /// <returns></returns>
        public async Task<string> RandomImageURLFromSplashSearch(string searchText)
        {
            string searchUrl = string.Concat("https://unsplash.com/s/photos/", searchText);
            string baseUrl = "https://unsplash.com";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(searchUrl);
            
            // Grab all the links with the photo url
            var images = doc.DocumentNode.SelectNodes("//a[@itemprop='contentUrl']")
                .Where(x => x.Attributes["href"].Value.StartsWith("/photos"))
                .ToList();

            int index = Random.Next(0, images.Count);

            string photoUrl = $"{baseUrl}{images[index].Attributes["href"].Value}";
            HtmlDocument photoPage = await web.LoadFromWebAsync(photoUrl);

            var items = photoPage.DocumentNode.SelectNodes("//img").ToList();

            if (items.Count > 1)
                return items[2].Attributes["src"].Value;

            return null;
        }

        private static Bitmap CropToBanner(Image image, int bannerWidth = 1100, int bannerHeight = 450)
        {
            // Get width / height
            var originalWidth = image.Width;
            var originalHeight = image.Height;
            
            // Convert background into size
            var destinationSize = new Size(bannerWidth, bannerHeight);
            
            // Ratios
            var heightRatio = (float)originalHeight / destinationSize.Height;
            var widthRatio = (float)originalWidth / destinationSize.Width;
            var ratio = Math.Min(heightRatio, widthRatio);
            
            // Scale
            var heightScale = Convert.ToInt32(destinationSize.Height * ratio);
            var widthScale = Convert.ToInt32(destinationSize.Width * ratio);

            var startX = (originalWidth - widthScale) / 2;
            var startY = (originalHeight - heightScale) / 2;

            var sourceRectangle = new Rectangle(startX, startY, widthScale, heightScale);
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Apply image
            using var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic; // highest quality possible
            g.DrawImage(image, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

            return bitmap;
        }
        
        private Image ClipImageToCircle(Image image)
        {
            Image destination = new Bitmap(image.Width, image.Height, image.PixelFormat);
            var radius = image.Width / 2;

            // Center
            var x = image.Width / 2;
            var y = image.Height / 2;

            using Graphics g = Graphics.FromImage(destination);
            var r = new Rectangle(x - radius, y - radius, radius * 2, radius * 2);
                
            // Use high quality settings
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
    
            using (Brush brush = new SolidBrush(Color.Transparent))
            {
                g.FillRectangle(brush, 0, 0, destination.Width, destination.Height);
            }

            // Apply the clipping
            var path = new GraphicsPath();
            path.AddEllipse(r);
            g.SetClip(path);
            g.DrawImage(image, 0, 0);
            
            return destination;
        }

        private Image CopyRegionIntoImage(Image source, Image destination)
        {
            using var graphicsDestination = Graphics.FromImage(destination);
            var x = (destination.Width / 2) - 110;
            var y = (destination.Height / 2) - 155;

            graphicsDestination.DrawImage(source, x, y, 220, 220);

            return destination;
        }

        private Color GetContrastColor(Color pixelColor)
        {
            var value = CalculateLuma(pixelColor);
            return value < 0.5 ? Color.White : Color.Black;
        }

        private float CalculateLuma(Color pixelColor)
        {
            return pixelColor.GetBrightness();
        }
        
        private Image DrawTextToImage(Image image, string header, string subheader)
        {
            var roboto = new Font("Roboto", 30, FontStyle.Regular);
            var robotoSmall = new Font("Roboto", 23, FontStyle.Regular);
            var sample = new Bitmap(image);
            
            // Coordinates for headers
            var headerX = image.Width / 2;
            var headerY = (image.Height / 2) + 115;

            var subheaderX = image.Width / 2;
            var subheaderY = (image.Height / 2) + 160;

            // Determine which text color is best suited for the given image
            var headerTextColor = GetContrastColor(sample.GetPixel(headerX, headerY));
            var subheaderTextColor = GetContrastColor(sample.GetPixel(subheaderX, subheaderY));
            
            // No longer need this -- dispose of it
            sample.Dispose();
            
            var drawFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            // Apply text to image
            using var graphicsDestination = Graphics.FromImage(image);
            graphicsDestination.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphicsDestination.DrawString(header, roboto, new SolidBrush(headerTextColor), headerX, headerY, drawFormat);
            graphicsDestination.DrawString(subheader, robotoSmall, new SolidBrush(subheaderTextColor), subheaderX, subheaderY, drawFormat);
            return new Bitmap(image);
        }
        
        private async Task<Image> FetchImageAsync(string url)
        {
            // Get image from URL
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            // Backup image -- should never NOT have an image
            if (!response.IsSuccessStatusCode)
                response = await client.GetAsync(_backupImageUrl);

            var stream = await response.Content.ReadAsStreamAsync();
            return Image.FromStream(stream);
        }
    }
}
using System.Threading.Tasks;

namespace ImageCreator.Services
{
    /// <summary>
    /// Service that will make finding images easier for our application
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Retrieve a random image based on the given query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<string> RandomImageUrl(string query);

        /// <summary>
        /// Url / Website utilized by this service -- used for credit purposes
        /// </summary>
        string BaseUrl { get; }
    }
}
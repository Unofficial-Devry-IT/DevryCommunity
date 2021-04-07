using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Contract for obtaining the currently-signed-in user
    /// </summary>
    public interface ICurrentUserService
    {
        string UserId { get; }
        Task<IdentityUser> CurrentUser();
    }
}
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        Task<ApplicationUser> CurrentUser();
    }
}
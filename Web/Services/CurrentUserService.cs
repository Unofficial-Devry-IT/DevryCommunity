using System.Security.Claims;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ApplicationUser> CurrentUser()
        {
            return await _context.Users.FindAsync(UserId);
        }
    }
}
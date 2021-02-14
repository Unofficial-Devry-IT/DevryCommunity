using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.CodeSnippets.Queries
{
    public class GetCodeSnippetsInCategoryQuery : IRequest<PaginatedList<CodeSnippet>>
    {
        public string[] CategoryIds { get; set; }
        public string[] CodeInfoIds { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCodeSnippetsInCategoryQueryHandler
        : IRequestHandler<GetCodeSnippetsInCategoryQuery, PaginatedList<CodeSnippet>>
    {
        private readonly IApplicationDbContext _context;
        private IMapper _mapper;

        public GetCodeSnippetsInCategoryQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<CodeSnippet>> Handle(GetCodeSnippetsInCategoryQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.CodeSnippets
                .Where(x => 
                               (request.CategoryIds != null && request.CategoryIds.Length > 0) ? request.CategoryIds.Contains(x.Id) : true 
                            && (request.CodeInfoIds != null && request.CodeInfoIds.Length > 0) ? request.CodeInfoIds.Contains(x.CodeInfoId) : true)
                .OrderBy(x => x.Category)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
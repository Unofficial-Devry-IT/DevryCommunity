using System.Threading;
using System.Threading.Tasks;
using Application.Channels.Commands;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.CodeSnippets.Commands
{
    public class UpdateCodeSnippetCommand : IRequest
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CodeInfoId { get; set; }
        public string Category { get; set; }
    }

    public class UpdateCodeSnippetCommandHandler : IRequestHandler<UpdateCodeSnippetCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCodeSnippetCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCodeSnippetCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CodeSnippets.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(CodeSnippet), request.Id);

            entity.Category = request.Category;
            entity.Description = request.Description;
            entity.CodeInfoId = request.CodeInfoId;
            entity.Content = request.Content;

            _context.CodeSnippets.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
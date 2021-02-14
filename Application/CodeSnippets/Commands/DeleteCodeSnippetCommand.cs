using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.CodeSnippets.Commands
{
    public class DeleteCodeSnippetCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteCodeSnippetCommandHandler : IRequestHandler<DeleteCodeSnippetCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCodeSnippetCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCodeSnippetCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CodeSnippets.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(CodeSnippet), request.Id);

            _context.CodeSnippets.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
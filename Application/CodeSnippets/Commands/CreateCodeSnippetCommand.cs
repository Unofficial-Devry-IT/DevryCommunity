using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events.CodeSnippets;
using MediatR;

namespace Application.CodeSnippets.Commands
{
    public class CreateCodeSnippetCommand : IRequest<string>
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CodeInfoId { get; set; }
    }

    public class CreateCodeSnippetCommandHandler : IRequestHandler<CreateCodeSnippetCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public CreateCodeSnippetCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateCodeSnippetCommand request, CancellationToken cancellationToken)
        {
            var entity = new CodeSnippet()
            {
                Created = DateTime.Now,
                Category = request.Category,
                Content = request.Content,
                CodeInfoId = request.CodeInfoId,
                Description = request.Description
            };
            
            entity.DomainEvents.Add(new CodeSnippetCreatedEvent(entity));

            await _context.CodeSnippets.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
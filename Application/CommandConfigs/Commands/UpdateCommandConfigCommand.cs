using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using Domain.Exceptions;
using MediatR;

namespace Application.CommandConfigs.Commands
{
    public class UpdateCommandConfigCommand : IRequest
    {
        public string Id { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public List<string> RestrictedRoles { get; set; } = new List<string>();
        public TimeSpan? TimeoutOverride { get; set; } = null;
        public string AuthorName { get; set; }
        public string AuthorIcon { get; set; }
        public string ReactionEmoji { get; set; }
        public string ExtensionData { get; set; }
        public string Description { get; set; }
    }

    public class UpdateCommandConfigHandler : IRequestHandler<UpdateCommandConfigCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCommandConfigHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCommandConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CommandConfigs.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(CommandConfig), request.Id);

            entity.IgnoreHelpWizard = request.IgnoreHelpWizard;
            entity.AuthorName = request.AuthorName;
            entity.TimeoutOverride = request.TimeoutOverride;
            entity.RestrictedRoles = request.RestrictedRoles;
            entity.AuthorIcon = request.AuthorIcon;
            entity.ReactionEmoji = request.ReactionEmoji;
            entity.ExtensionData = request.ExtensionData;
            entity.Description = request.Description;

            _context.CommandConfigs.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
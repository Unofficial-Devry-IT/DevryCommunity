using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using Domain.Exceptions;
using MediatR;

namespace Application.WizardConfigs.Commands
{
    public class UpdateWizardConfigCommand : IRequest
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public bool AcceptAnyUser { get; set; }
        public bool RequireMention { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public List<string> RestrictedRoles { get; set; } = new List<string>();
    }

    public class UpdateWizardConfigCommandHandler : IRequestHandler<UpdateWizardConfigCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateWizardConfigCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateWizardConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WizardConfigs.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(WizardConfig), request.Id);

            entity.Headline = request.Headline;
            entity.AcceptAnyUser = request.AcceptAnyUser;
            entity.RequireMention = request.RequireMention;
            entity.IgnoreHelpWizard = request.IgnoreHelpWizard;
            entity.RestrictedRoles = request.RestrictedRoles;

            _context.WizardConfigs.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
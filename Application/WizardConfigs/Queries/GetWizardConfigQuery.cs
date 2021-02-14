using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using Domain.Exceptions;
using MediatR;

namespace Application.WizardConfigs.Queries
{
    public class GetWizardConfigQuery : IRequest<WizardConfig>
    {
        public string Id { get; set; }
    }

    public class GetWizardConfigQueryHandler : IRequestHandler<GetWizardConfigQuery, WizardConfig>
    {
        private readonly IApplicationDbContext _context;

        public GetWizardConfigQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WizardConfig> Handle(GetWizardConfigQuery request, CancellationToken cancellationToken)
        {
            return await _context.WizardConfigs.FindAsync(request.Id);
        }
    }
}
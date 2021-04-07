using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Configs.Commands
{
    public class CreateConfigCommand : IRequest<string>
    {
        public string ConfigName { get; set; }
        public ConfigType ConfigType { get; set; }
        public string Data { get; set; }
    }

    public class CreateConfigCommandHandler : IRequestHandler<CreateConfigCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public CreateConfigCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Configs.FirstOrDefaultAsync(x =>
                x.ConfigName.Equals(request.ConfigName, StringComparison.CurrentCultureIgnoreCase));

            if (entity != null)
            {
                entity.ConfigType = request.ConfigType;
                entity.ExtensionData = request.Data.FromBase64();

                _context.Configs.Update(entity);
            }
            else
            {
                entity = new Config()
                {
                    ConfigName = request.ConfigName,
                    ConfigType = request.ConfigType,
                    ExtensionData = request.Data
                };

                await _context.Configs.AddAsync(entity, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            return entity.Id.ToString();
        }
    }
}
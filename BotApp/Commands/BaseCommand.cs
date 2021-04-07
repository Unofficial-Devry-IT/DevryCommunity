using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BotApp.Commands
{
    public abstract class BaseCommand : BaseCommandModule, IDiscordCommand
    {
        /// <summary>
        /// What shall be called when command is 'executed'
        /// </summary>
        /// <param name="context"></param>
        public abstract Task ExecuteAsync(CommandContext context);

        public Config CurrentConfig { get; protected set; }

        private ISender _mediator;
        protected ISender Mediator => _mediator ??= Bot.Instance?.ServiceProvider.GetService<ISender>();
        public async Task<Config> FindConfig()
        {
            /*
             *  Technically our interaction configs will be invoked via
             *  Commands. Plus, our interaction configs are derived from Command configs
             *
             *  - So first - check to see if we get a config that matches the prefix of our command
             *  - If nothing is found then we search for a command config with this type's name
             */
            string prefix = GetType().Name.Replace("Command", "");

            var configs = await Bot.Instance.Context.Configs
                .Where(x => x.ConfigType == ConfigType.INTERACTION &&
                            x.ConfigName.StartsWith(prefix))
                .ToListAsync();

            if (configs.Any())
                return configs.First();
            
            return await Bot.Instance.Context.Configs
                .Where(x => x.ConfigType == ConfigType.COMMAND &&
                            x.ConfigName.Equals(GetType().Name))
                .FirstOrDefaultAsync();
        }
    }
}
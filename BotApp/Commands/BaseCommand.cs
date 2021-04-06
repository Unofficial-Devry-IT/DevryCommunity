using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Configs;
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
            MethodInfo commandMethod = GetType().GetMethod("ExecuteAsync");

            if (commandMethod == null)
                throw new NotFoundException(nameof(Config), "ExecuteAsync Method not found");

            var attribute = commandMethod.GetCustomAttribute<CommandAttribute>();

            if (attribute == null)
            {
                Bot.Instance.Logger.LogError($"{GetType().Name}.ExecuteAsync does not have CommandAttribute");
                throw new NotFoundException(nameof(Config), "Command Attribute not found");
            }

            return await Bot.Instance.Context.Configs
                .FirstOrDefaultAsync(x =>
                    x.ConfigName.Equals(attribute.Name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
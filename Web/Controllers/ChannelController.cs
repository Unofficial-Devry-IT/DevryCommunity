using System.Threading.Tasks;
using Application.Channels.Commands;
using Application.Channels.Queries;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Handles the CRUD events pertaining to <see cref="Domain.Entities.Discord.Channel"/>
    /// </summary>
    [Authorize]
    [ApiController]
    public class ChannelController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<ChannelResponse>>> GetChannelsWithPagination(
            [FromQuery] GetAllChannelsPaginatedQuery query)
        {
            return await Mediator.Send(query);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(ulong id, UpdateChannelCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateChannelDetails(ulong id, UpdateChannelCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(ulong id)
        {
            await Mediator.Send(new DeleteChannelCommand() {Id = id});
            return NoContent();
        }
    }
}
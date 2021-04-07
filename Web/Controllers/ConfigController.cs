using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Configs.Commands;
using Application.Configs.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Handles the CRUD events for <see cref="Domain.Entities.Config"/>
    /// </summary>
    [Authorize]
    [ApiController]
    public class ConfigController : ApiControllerBase
    {
        [HttpGet]
        [Route("configs")]
        public async Task<ActionResult<PaginatedList<Config>>> GetAllConfigs(
            [FromQuery] GetAllConfigsPaginatedQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet]
        public async Task<ActionResult<Config>> Get([FromBody] GetConfigQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] CreateConfigCommand query)
        {
            await Mediator.Send(query);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromQuery] UpdateConfigCommand query)
        {
            await Mediator.Send(query);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteConfigCommand query)
        {
            await Mediator.Send(query);
            return Ok();
        }
    }
}
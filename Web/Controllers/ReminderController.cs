using System.Threading.Tasks;
using Application.Common.Models;
using Application.Reminders.Commands;
using Application.Reminders.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Handles CRUD events for <see cref="Domain.Entities.Reminder"/>
    /// </summary>
    [Authorize]
    [ApiController]
    public class ReminderController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<Reminder>>> GetRemindersWithPagination(
            [FromQuery] GetRemindersPaginatedQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<string>> CreateReminder([FromBody] CreateReminderCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
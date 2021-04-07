using System.Threading.Tasks;
using BotApp.Classes.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class CourseController : ApiControllerBase
    {
        private readonly ILogger<CourseController> _logger;

        public CourseController(ILogger<CourseController> logger)
        {
            _logger = logger;
        }
        
        [HttpPost]
        [Route("create-course")]
        public async Task<ActionResult> CreateCourse([FromBody]CreateClassCommand request)
        {
            _logger.LogInformation(request.ToString());
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpPost]
        [Route("create-public")]
        public async Task<ActionResult> CreatePublic()
        {
            return NoContent();
        }

        [HttpPost]
        [Route("create-resource")]
        public async Task<ActionResult> CreateResource()
        {
            return NoContent();
        }
    }
}
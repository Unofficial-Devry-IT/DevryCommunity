using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        private ISender _mediator;
        
        /// <summary>
        /// Acquire the mediator service which shall be utilized by all controllers in this assembly
        /// </summary>
        public ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
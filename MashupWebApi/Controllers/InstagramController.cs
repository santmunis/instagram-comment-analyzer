using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MashupWebApi.Domain.Aggregates.Instagram.Queries;
using MashupWebApi.Extesions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace MashupWebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class InstagramController : Controller
    {
        private readonly IMediator _mediator;

        public InstagramController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publication/comments")]
        public async Task<IActionResult> GetCommentByPublicationUrl([FromBody] GetCommentsByPublications.Contract request)
        {
            var result = await _mediator.Send(request);
            return RestResult.CreateHttpResponse(result);
        }
    }
}

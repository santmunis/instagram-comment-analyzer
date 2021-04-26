using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MashupWebApi.Domain.Aggregates.Instagram.Queries;
using MashupWebApi.Domain.Aggregates.Watson.Queries;
using MashupWebApi.Extesions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace MashupWebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class WatsonController : Controller
    {
        private readonly IMediator _mediator;
        public WatsonController(IMediator mediator)
        {
                _mediator = mediator;
        }

        [HttpPost("analyze/feeling/text")]
        public async Task<IActionResult> GetTextFeeling([FromBody] GetTextFeeling.Contract request)
        {
            var result = await _mediator.Send(request);
            return RestResult.CreateHttpResponse(result);
        }
        
    }
}

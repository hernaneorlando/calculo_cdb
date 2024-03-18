using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.CalculoCdb;

[ApiController]
[Route("[controller]")]
public class CalculoCdbController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CalculeCdb(CalculoCdbCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

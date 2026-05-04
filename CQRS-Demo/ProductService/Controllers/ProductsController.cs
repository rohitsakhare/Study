using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;

namespace ProductService.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand cmd)
        => Ok(await _mediator.Send(cmd));
}
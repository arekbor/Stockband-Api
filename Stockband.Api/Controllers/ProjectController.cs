using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class ProjectController:ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("project")]
    public async Task<IActionResult> CreateProject(CreateProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("project")]
    public async Task<IActionResult> UpdateProject(UpdateProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete]
    [Route("project")]
    public async Task<IActionResult> RemoveProject(RemoveProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;
using Stockband.Domain.Common;

namespace Stockband.Api.Controllers;

[Authorize]
[ApiController]
public class ProjectController:ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("/project")]
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
    [Route("/project")]
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
    [Route("/project")]
    public async Task<IActionResult> RemoveProject(RemoveProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet]
    [Route("/projects/{userId:int}")]
    public async Task<IActionResult> GetAllUserProjects(int userId)
    {
        BaseResponse<List<GetAllUserProjectsQueryViewModel>> response = 
            await _mediator.Send(new GetAllUserProjectsQuery
        {
            UserId = userId
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
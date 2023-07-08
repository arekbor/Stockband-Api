using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Api.Dtos.Project;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class ProjectController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationUserService _authenticationUserService;
    public ProjectController(IMediator mediator, IAuthenticationUserService authenticationUserService)
    {
        _mediator = mediator;
        _authenticationUserService = authenticationUserService;
    }

    [HttpPost]
    [Route("project")]
    public async Task<IActionResult> CreateProject(CreateProjectDto createProjectDto)
    {
        BaseResponse response = await _mediator.Send(new CreateProjectCommand
        {
            RequestedUserId = _authenticationUserService.GetCurrentUserId(),
            ProjectName = createProjectDto.ProjectName,
            ProjectDescription = createProjectDto.ProjectDescription
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("project")]
    public async Task<IActionResult> UpdateProject(UpdateProjectDto updateProjectDto)
    {
        BaseResponse response = await _mediator.Send(new UpdateProjectCommand
        {
            RequestedUserId = _authenticationUserService.GetCurrentUserId(),
            ProjectId = updateProjectDto.ProjectId,
            ProjectName = updateProjectDto.ProjectName,
            ProjectDescription = updateProjectDto.ProjectDescription
        });
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete]
    [Route("project")]
    public async Task<IActionResult> RemoveProject(RemoveProjectDto removeProjectDto)
    {
        BaseResponse response = await _mediator.Send(new RemoveProjectCommand
        {
            RequestedUserId = _authenticationUserService.GetCurrentUserId(),
            ProjectId = removeProjectDto.ProjectId
        });
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
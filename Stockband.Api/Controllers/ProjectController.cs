using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Api.Dtos.Project;
using Stockband.Api.Interfaces;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class ProjectController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationUser _authorizationUser;
    public ProjectController(IMediator mediator, IAuthorizationUser authorizationUser)
    {
        _mediator = mediator;
        _authorizationUser = authorizationUser;
    }

    [HttpPost]
    [Route("project")]
    public async Task<IActionResult> CreateProject(CreateProjectDto createProjectDto)
    {
        BaseResponse response = await _mediator.Send(new CreateProjectCommand
        {
            OwnerId = _authorizationUser.GetUserIdFromClaims(),
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
            OwnerId = _authorizationUser.GetUserIdFromClaims(),
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
}
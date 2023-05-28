using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Api.Dtos.ProjectMember;
using Stockband.Api.Interfaces;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class ProjectMemberController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationUser _authorizationUser;

    public ProjectMemberController(IMediator mediator, IAuthorizationUser authorizationUser)
    {
        _mediator = mediator;
        _authorizationUser = authorizationUser;
    }

    [HttpPost]
    [Route("projectMember")]
    public async Task<IActionResult> AddProjectMemberToProject(AddProjectMemberDto addProjectMemberDto)
    {
        BaseResponse response = await _mediator.Send(new AddProjectMemberToProjectCommand
        {
            ProjectOwnerId = _authorizationUser.GetUserIdFromClaims(),
            ProjectId = addProjectMemberDto.ProjectId,
            MemberId = addProjectMemberDto.MemberId
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete]
    [Route("projectMember")]
    public async Task<IActionResult> RemoveProjectMemberFromProject(RemoveProjectMemberDto removeProjectMemberDto)
    {
        BaseResponse response = await _mediator.Send(new RemoveMemberFromProjectCommand
        {
            ProjectOwnerId = _authorizationUser.GetUserIdFromClaims(),
            ProjectId = removeProjectMemberDto.ProjectId,
            MemberId = removeProjectMemberDto.MemberId
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
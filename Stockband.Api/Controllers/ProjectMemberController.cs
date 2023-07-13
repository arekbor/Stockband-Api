using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;
using Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;
using Stockband.Domain.Common;

namespace Stockband.Api.Controllers;

[Authorize]
[ApiController]
public class ProjectMemberController:ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [Route("projectMember")]
    public async Task<IActionResult> AddProjectMemberToProject(AddProjectMemberToProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete]
    [Route("projectMember")]
    public async Task<IActionResult> RemoveProjectMemberFromProject(RemoveMemberFromProjectCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    [HttpGet]
    [Route("projectMember/{projectId:int}")]
    public async Task<IActionResult> GetAllProjectMembers(int projectId)
    {
        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response =
            await _mediator.Send(new GetAllProjectMembersQuery(projectId));

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.UserFeatures.Commands.LoginUser;
using Stockband.Application.Features.UserFeatures.Commands.LogoutUser;
using Stockband.Application.Features.UserFeatures.Commands.RegisterUser;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Application.Features.UserFeatures.Commands.UpdateRole;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Application.Features.UserFeatures.Queries.GetUserById;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[Authorize]
[ApiController]
public class UserController:ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/user/{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        BaseResponse<GetUserByIdQueryViewModel> response =  await _mediator.Send(new GetUserByIdQuery
        {
            Id = id
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("/user/register")]
    public async Task<IActionResult> UserRegister(RegisterUserCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("/user/login")]
    public async Task<IActionResult> UserLogin(LoginUserCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost]
    [Route("/user/logout")]
    public async Task<IActionResult> UserLogout()
    {
        BaseResponse response = await _mediator.Send(new LogoutUserCommand());
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("/user/password")]
    public async Task<IActionResult> UserUpdatePassword(UpdatePasswordCommand command)
    {
        BaseResponse response =  await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("/user/update")]
    public async Task<IActionResult> UserUpdate(UpdateUserCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    [Route("/user/role")]
    public async Task<IActionResult> UserRole(UpdateRoleCommand command)
    {
        BaseResponse response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
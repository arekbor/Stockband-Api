using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Api.Dtos.User;
using Stockband.Api.Interfaces;
using Stockband.Application.Features.UserFeatures.Commands.RegisterUser;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Application.Features.UserFeatures.Commands.UpdateRole;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;
using Stockband.Application.Features.UserFeatures.Queries.GetUserById;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class UserController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationUser _authorizationUser;
    public UserController(IMediator mediator, IAuthorizationUser authorizationUser)
    {
        _mediator = mediator;
        _authorizationUser = authorizationUser;
    }

    [HttpGet]
    [Route("/user/{id:int}")]
    [Authorize(Roles = "Admin")]
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
    [Route("/user/register")]
    [AllowAnonymous]
    public async Task<IActionResult> UserRegister(RegisterUserDto registerUserDto)
    {
        BaseResponse response = await _mediator.Send(new RegisterUserCommand
        {
            Username = registerUserDto.Username,
            Email = registerUserDto.Email,
            Password = registerUserDto.Password,
            ConfirmPassword = registerUserDto.ConfirmPassword
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost]
    [Route("/user/login")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLogin(LoginUserDto loginUserDto)
    {
        BaseResponse<GetLoggedUserQueryViewModel> response = await _mediator.Send(new GetLoggedUserQuery
        {
            Email = loginUserDto.Email,
            Password = loginUserDto.Password
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }

        GetLoggedUserQueryViewModel viewModel = response.Result;

        string jwtToken = _authorizationUser.CreateJwt
        (
            viewModel.Id.ToString(), 
            viewModel.Username, 
            viewModel.Email,
            viewModel.Role.ToString()
        );
        _authorizationUser.AddJwtCookie(jwtToken);
        
        return Ok(response);
    }

    [HttpPost]
    [Route("/user/logout")]
    public IActionResult UserLogout()
    {
        _authorizationUser.ClearJwtCookie();
        return NoContent();
    }

    [HttpPut]
    [Route("/user/password")]
    public async Task<IActionResult> UserUpdatePassword(UpdateUserPasswordDto updateUserPasswordDto)
    {
        BaseResponse response =  await _mediator.Send(new UpdatePasswordCommand
        {
            RequestedUserId = _authorizationUser.GetUserIdFromClaims(),
            CurrentPassword = updateUserPasswordDto.CurrentPassword,
            NewPassword = updateUserPasswordDto.NewPassword,
            ConfirmNewPassword = updateUserPasswordDto.ConfirmNewPassword
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("/user/update")]
    public async Task<IActionResult> UserUpdate(UpdateUserDto updateUserDto)
    {
        BaseResponse response = await _mediator.Send(new UpdateUserCommand
        {
            RequestedUserId = _authorizationUser.GetUserIdFromClaims(),
            UserId = updateUserDto.UserId,
            Username = updateUserDto.Username,
            Email = updateUserDto.Email,
        });
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut]
    [Route("/user/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserRole(UpdateRoleDto updateRoleDto)
    {
        BaseResponse response = await _mediator.Send(new UpdateRoleCommand
        {
            RequestedUserId = _authorizationUser.GetUserIdFromClaims(),
            UserId = updateRoleDto.UserId,
            Role = updateRoleDto.Role
        });

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
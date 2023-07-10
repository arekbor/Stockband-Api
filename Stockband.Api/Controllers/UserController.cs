using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.UserFeatures.Commands.RegisterUser;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Application.Features.UserFeatures.Commands.UpdateRole;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;
using Stockband.Application.Features.UserFeatures.Queries.GetUserById;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;

namespace Stockband.Api.Controllers;

[ApiController]
[Authorize]
public class UserController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationUserService _authenticationUserService;
    public UserController(IMediator mediator, IAuthenticationUserService authenticationUserService)
    {
        _mediator = mediator;
        _authenticationUserService = authenticationUserService;
    }

    [HttpGet]
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
    [Route("/user/register")]
    [AllowAnonymous]
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
    [Route("/user/login")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLogin(GetLoggedUserQuery query)
    {
        BaseResponse<GetLoggedUserQueryViewModel> response = await _mediator.Send(query);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        GetLoggedUserQueryViewModel viewModel = response.Result;

        string jwtToken = _authenticationUserService.GenerateJwtToken
        (
            viewModel.Id.ToString(), 
            viewModel.Username, 
            viewModel.Email,
            viewModel.Role.ToString()
        );
        _authenticationUserService.AddJwtCookie(jwtToken);
        
        return Ok(response);
    }

    [HttpPost]
    [Route("/user/logout")]
    public IActionResult UserLogout()
    {
        _authenticationUserService.ClearJwtCookie();
        return NoContent();
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
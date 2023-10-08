using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockband.Application.Features.TrackFeature.Queries.GetTrackGenresDictionary;
using Stockband.Application.Features.TrackFeature.Queries.GetTrackTypesDictionary;
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Api.Controllers;

[Authorize]
[ApiController]
public class TrackController:ControllerBase
{
    private readonly IMediator _mediator;

    public TrackController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Route("/track/getTrackGenresDictionary")]
    public async Task<IActionResult> GetTrackGenresDictionary()
    {
        BaseResponse<List<DictionaryResponse<TrackGenre>>> response =
            await _mediator.Send(new GetTrackGenresDictionaryQuery());

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    [HttpGet]
    [Route("/track/getTrackTypesDictionary")]
    public async Task<IActionResult> GetTrackTypesDictionary()
    {
        BaseResponse<List<DictionaryResponse<TrackType>>> response =
            await _mediator.Send(new GetTrackTypesDictionaryQuery());

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
using MediatR;
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Application.Features.TrackFeature.Queries.GetTrackTypesDictionary;

public class GetTrackTypesDictionaryQuery:IRequest<BaseResponse<List<DictionaryResponse<TrackType>>>>
{
    
}
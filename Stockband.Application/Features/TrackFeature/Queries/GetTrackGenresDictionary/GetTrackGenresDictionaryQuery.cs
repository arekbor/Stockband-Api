using MediatR;
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Application.Features.TrackFeature.Queries.GetTrackGenresDictionary;

public class GetTrackGenresDictionaryQuery:IRequest<BaseResponse<List<DictionaryResponse<TrackGenre>>>>
{
    
}
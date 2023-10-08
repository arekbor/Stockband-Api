using MediatR;
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Application.Features.TrackFeature.Queries.GetTrackGenresDictionary;

public class GetTrackGenresDictionaryQueryHandler : 
    IRequestHandler<GetTrackGenresDictionaryQuery, BaseResponse<List<DictionaryResponse<TrackGenre>>>>
{
    public async Task<BaseResponse<List<DictionaryResponse<TrackGenre>>>> 
        Handle(GetTrackGenresDictionaryQuery request, CancellationToken cancellationToken)
    {
        List<DictionaryResponse<TrackGenre>> dictionaryResponses = 
            DictionaryResponse<TrackGenre>.GetEnumDictionary();
        
        return new BaseResponse<List<DictionaryResponse<TrackGenre>>>(dictionaryResponses);
    }
}
using MediatR;
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Application.Features.TrackFeature.Queries.GetTrackTypesDictionary;

public class GetTrackTypesDictionaryQueryHandler:
    IRequestHandler<GetTrackTypesDictionaryQuery, BaseResponse<List<DictionaryResponse<TrackType>>>>
{
    public async Task<BaseResponse<List<DictionaryResponse<TrackType>>>> 
        Handle(GetTrackTypesDictionaryQuery request, CancellationToken cancellationToken)
    {
        List<DictionaryResponse<TrackType>> dictionaryResponses = 
            DictionaryResponse<TrackType>.GetEnumDictionary();
        
        return new BaseResponse<List<DictionaryResponse<TrackType>>>(dictionaryResponses);
    }
}
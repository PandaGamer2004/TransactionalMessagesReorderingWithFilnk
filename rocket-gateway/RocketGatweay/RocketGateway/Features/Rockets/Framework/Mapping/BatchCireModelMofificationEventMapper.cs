using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Framework.Mapping;

public class BatchCireModelModificationEventMapper: IMapper<BatchedModificationEvent, OperationResult<BatchedCoreModificationEvent, ErrorModel>>
{
    private readonly IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> _singleModelMapper;

    public BatchCireModelModificationEventMapper(
        IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> singleModelMapper
            )
    {
        _singleModelMapper = singleModelMapper;
    }
    public OperationResult<BatchedCoreModificationEvent, ErrorModel> Map(BatchedModificationEvent input)
    {
        var traversedMapping =  OperationResult<RocketChangeCoreEvent, ErrorModel>.
            Travere(input.BatchedModels, _singleModelMapper.Map);

        return traversedMapping.Map(mappedDomainEvents => new BatchedCoreModificationEvent
        {
            ChannelId = input.ChannelId,
            RocketChangeDomainEvents = mappedDomainEvents
        });
    }
}
using System.Text.Json;
using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Models.Messages;
using RocketGateway.Features.Rockets.Models;
using RocketGateway.Features.Rockets.Models.Events;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Mapping;

public class RocketChangeEventMapper: IMapper<RocketChangeEvent, OperationResult<RocketChangeDomainEvent, ErrorModel>>
{
    private static readonly Dictionary<RocketMessageType, Type> RocketMessageMappingConventions = new()
    {
        { RocketMessageType.RocketSpeedIncreased, typeof(RocketSpeedChangeMessage) },
        { RocketMessageType.RocketSpeedDecreased, typeof(RocketSpeedChangeMessage) },
        { RocketMessageType.RocketMissionChanged, typeof(RocketMissionChangedMessage) },
        { RocketMessageType.RocketExploded, typeof(RocketExplodedMessage) },
        { RocketMessageType.RocketLaunched, typeof(RocketLaunchedMessage) }
    };

    public OperationResult<RocketChangeDomainEvent, ErrorModel> Map(RocketChangeEvent rocketChangeEvent)
    {
        var eventType = rocketChangeEvent.Metadata.MessageType;
        return OperationResult<string, ErrorModel>.Unit(eventType)
            .FlatMap(eventType =>
            {
                if (Enum.TryParse<RocketMessageType>(eventType, out var mappedEventType))
                {
                    return OperationResult<RocketMessageType, ErrorModel>
                        .CreateSuccess(mappedEventType);
                }

                return OperationResult<RocketMessageType, ErrorModel>.CreateError(
                    new ErrorModel
                    {
                        ErrorCode = ErrorCodes.FailedToMapEventType,
                        ErrorDescription = "Invalid event type was passed"
                    });
            })
            .FlatMap(mappedEventType =>
            {
                var messageType = RocketMessageMappingConventions[mappedEventType];
                try
                {
                    IRocketMessage? castedMessage = (IRocketMessage?)JsonSerializer.Deserialize(
                        rocketChangeEvent.Message,
                        messageType, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    if (castedMessage == null)
                    {
                        return OperationResult<RocketChangeDomainEvent, ErrorModel>.CreateError(
                            new ErrorModel
                            {
                                ErrorCode = ErrorCodes.InvalidPayloadForEventType,
                                ErrorDescription = "Failed to parse passed message"
                            });
                    }

                    return OperationResult<RocketChangeDomainEvent, ErrorModel>.CreateSuccess(
                        new RocketChangeDomainEvent
                        {
                            Message = castedMessage,
                            Metadata = RocketEventDomainMetadata.FromMetadata(
                                rocketChangeEvent.Metadata,
                                mappedEventType)
                        }
                    );
                }
                catch (Exception ex)
                {
                    return OperationResult<RocketChangeDomainEvent, ErrorModel>.CreateError(new ErrorModel
                    {
                        ErrorCode = ErrorCodes.InvalidPayloadForEventType,
                        ErrorDescription = "Failed to parse passed message"
                    });
                }
            });
    }
}
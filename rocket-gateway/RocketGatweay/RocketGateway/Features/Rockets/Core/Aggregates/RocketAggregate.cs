using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Models.Messages;
using RocketGateway.Features.Rockets.Core.State;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Core.Aggregates;

//Because we have total order enforeced on the events, and thank u kafka:) we have each event batch delivered
//In order to their consumer we don't have any need in optimistic locks
public class RocketAggregate
{
    //By fact is aggregate id
    private RocketId Id { get; set; }

    private string Type { get; set; }

    private int LaunchSpeed { get; set; }

    private string Mission { get; set; }

    private string ExplodedReason { get; set; }

    private bool WasExploded { get; set; }


    public RocketAggregateState State => new RocketAggregateState
    {
        Id = Id,
        ExplodedReason = ExplodedReason,
        WasExploded = WasExploded,
        LaunchSpeed = LaunchSpeed,
        Mission = Mission,
        Type = Type
    };

    public static RocketAggregate FromExternalState(RocketAggregateState state)
        => new RocketAggregate
        {
            Id = state.Id,
            ExplodedReason = state.ExplodedReason,
            WasExploded = state.WasExploded,
            LaunchSpeed = state.LaunchSpeed,
            Mission = state.Mission,
            Type = state.Type,
        };
    
    //Should i call it events? Or commands are more suitable for it
    public OperationResult<VoidResult, string> Apply(RocketChangeCoreEvent rocketChangeCoreEvent)
    {
        if (WasExploded)
        {
            return OperationResult<VoidResult, string>
                .CreateError("Unable to apply any events on exploded rocket");
        }
        
        var messageType = rocketChangeCoreEvent.Metadata.MessageType;
        if (messageType == RocketMessageType.RocketLaunched)
        {
            //It's app invariant
            var launchedMessage = rocketChangeCoreEvent.Message as RocketLaunchedMessage;

            if (launchedMessage is null)
            {
                return BrokenInvariantResult;
            }
            
            if (this.Id.IsInitialized)
            {
                return OperationResult<VoidResult, string>.CreateError(
                    "Impossible to create rocket with already existing channel"
                );
            }

            this.Id = RocketId.FromValue(rocketChangeCoreEvent.Metadata.Channel);
            this.LaunchSpeed = launchedMessage.LaunchSpeed;
            this.Type = launchedMessage.Type;
            this.Mission = launchedMessage.Mission;
            return OperationResult<VoidResult, string>.Unit(VoidResult.Instance);
        }

        if (messageType == RocketMessageType.RocketSpeedIncreased)
        {
            var change = rocketChangeCoreEvent.Message as RocketSpeedChangeMessage;

            if (change is null)
            {
                return BrokenInvariantResult;
            }

            return OperationResult<Int32, string>.FromThrowableOperation(() =>
            {
                checked
                {
                    return this.LaunchSpeed + change.By;
                }
            }, err =>
            {
                if (err is OverflowException ex)
                {
                    return $"Speed can't exceed {Int32.MaxValue}";
                }

                return "Failure was faced during speed change";
            }).Map(res =>
            {
                this.LaunchSpeed = res;
                return VoidResult.Instance;
            });
        }

        if (messageType == RocketMessageType.RocketSpeedDecreased)
        {
            var change = rocketChangeCoreEvent.Message as RocketSpeedChangeMessage;

            if (change is null)
            {
                return BrokenInvariantResult;
            }

            var possibleLaunchSpeed = this.LaunchSpeed - change.By;

            if (possibleLaunchSpeed <= 0)
            {
                return OperationResult<VoidResult, string>
                    .CreateError("Unable to change rocket speed. It shouldn't be less or equal to 0");
            }
            
            this.LaunchSpeed -= change.By;
            return OperationResult<VoidResult, string>.Unit(VoidResult.Instance);
        }

        if (messageType == RocketMessageType.RocketMissionChanged)
        {
            var castedMissionChange = rocketChangeCoreEvent.Message as RocketMissionChangedMessage;
            if (castedMissionChange == null)
            {
                return BrokenInvariantResult;
            }
            
            this.Mission = castedMissionChange.NewMission;
            
            return OperationResult<VoidResult, string>.Unit(VoidResult.Instance);
        }

        if (messageType == RocketMessageType.RocketExploded)
        {
            var castedExplodedMessage = rocketChangeCoreEvent.Message as RocketExplodedMessage;

            if (castedExplodedMessage == null)
            {
                return BrokenInvariantResult;
            }
            
            this.WasExploded = true;
            this.ExplodedReason = castedExplodedMessage.Reason;
        }
        
        return OperationResult<VoidResult, String>
            .CreateError("Failed to match message type against known");
    }
    
    private OperationResult<VoidResult, string> BrokenInvariantResult
        => OperationResult<VoidResult, string>
            .CreateError($"Broken system variants on entity {nameof(RocketAggregate)}");
    
}
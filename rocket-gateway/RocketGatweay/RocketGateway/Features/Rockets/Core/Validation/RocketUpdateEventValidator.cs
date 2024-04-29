using System.ComponentModel.DataAnnotations;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Models.Messages;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Validation;

namespace RocketGateway.Features.Rockets.Core.Validation;

public class RocketUpdateEventValidator: IValidator<RocketChangeCoreEvent>
{
    public ValidationResult<string> Validate(RocketChangeCoreEvent model)
    {
        var message = model.Message;
        if (message is RocketExplodedMessage explodedMessage)
        {
            return Validate(explodedMessage);
        }

        if (message is RocketLaunchedMessage launchedMessage)
        {
            return Validate(launchedMessage);
        }

        if (message is RocketMissionChangedMessage missionChangedMessage)
        {
            return Validate(missionChangedMessage);
        }

        if (message is RocketSpeedChangeMessage speedChangeMessage)
        {
            return Validate(speedChangeMessage);
        }

        return ValidationResult<string>.CreateError("Failed to match message type against known");
    }
    
    
    private static ValidationResult<string> Validate(RocketExplodedMessage explodedMessage)
    {
        if (explodedMessage.Reason == null)
        {
            return ValidationResult<string>
                .CreateError("Reason should be supplied");
        }

        return ValidationResult<string>
            .CreateSuccess();
    }
    
    
    
    private static ValidationResult<string> Validate(RocketLaunchedMessage launchedMessage)
    {
        if (launchedMessage.Type == null)
        {
            return ValidationResult<string>
                .CreateError("Type can't be null");
        }

        if (launchedMessage.Mission == null)
        {
            return ValidationResult<string>
                .CreateError("Mission can't be null");
        }

        return ValidationResult<string>
            .CreateSuccess();
    }
    
    public ValidationResult<string> Validate(RocketMissionChangedMessage missionChangedMessage)
    {
        if (missionChangedMessage.NewMission == null)
        {
            return ValidationResult<string>.CreateError(
                "NewMission can't be null"
            );
        }

        return ValidationResult<string>.CreateSuccess();
    }
    
    public static ValidationResult<string> Validate(RocketSpeedChangeMessage message)
    {
        if (message.By < 0)
        {
            return ValidationResult<string>
                .CreateError("By can't be less than 0");
        }

        return ValidationResult<string>
            .CreateSuccess();
    }
}
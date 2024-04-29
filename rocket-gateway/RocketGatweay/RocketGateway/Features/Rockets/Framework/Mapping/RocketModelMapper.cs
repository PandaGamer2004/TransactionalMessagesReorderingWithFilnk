using RocketGateway.Features.Rockets.Core.State;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Shared.Mapping.Interfaces;

namespace RocketGateway.Features.Rockets.Framework.Mapping;

public class RocketModelMapper: IMapper<RocketAggregateState, ResponseRocketModel>
{
    public ResponseRocketModel Map(RocketAggregateState input)
        => new ResponseRocketModel
        {
            Id = input.Id.Value,
            Mission = input.Mission,
            Type = input.Type,
            ExplodedReason = input.ExplodedReason,
            LaunchSpeed = input.LaunchSpeed,
            WasExploded = input.WasExploded,
        };

}
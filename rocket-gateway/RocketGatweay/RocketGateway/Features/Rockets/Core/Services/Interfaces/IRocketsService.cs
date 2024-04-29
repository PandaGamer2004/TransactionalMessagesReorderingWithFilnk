using RocketGateway.Features.Rockets.Core.Aggregates;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Core.Services.Interfaces;

public interface IRocketsService
{
    public Task<OperationResult<VoidResult, string>> ProcessUnfilteredEvent(
        RocketChangeCoreEvent rocketChangeCoreEvent,
        CancellationToken ct = default);

    public Task<OperationResult<VoidResult, string>> ProcessFilteredEvent(
        BatchedCoreModificationEvent batchedCoreModificationEvent,
        CancellationToken ct = default
    );

    public Task<OperationResult<RocketAggregate, string>> GetRocketById(RocketId rocketId, CancellationToken ct = default);

    public Task<OperationResult<IEnumerable<RocketAggregate>, string>> GetRockets(CancellationToken ct = default);
}
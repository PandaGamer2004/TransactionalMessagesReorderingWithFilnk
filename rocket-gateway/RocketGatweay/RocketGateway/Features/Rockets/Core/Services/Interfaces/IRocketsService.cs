using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Core.Services.Interfaces;

public interface IRocketsService
{
    public Task<OperationResult<VoidResult, string>> ProcessUnfilteredEvent(
        RocketChangeDomainEvent rocketChangeDomainEvent,
        CancellationToken ct = default);

    public Task<OperationResult<VoidResult, string>> ProcessFilteredEventBatch(
        FilteredEventsBatch eventsBatch,
        CancellationToken ct = default);

}
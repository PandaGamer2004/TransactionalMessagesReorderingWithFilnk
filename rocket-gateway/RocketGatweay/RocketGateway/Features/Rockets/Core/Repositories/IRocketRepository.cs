using RocketGateway.Features.Rockets.Core.Aggregates;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Repositories.Models;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Core.Repositories;

public interface IRocketRepository
{
    Task<OperationResult<IEnumerable<RocketAggregate>, string>> Query(QuerySpec querySpec);
    Task<OperationResult<RocketAggregate, string>> LoadRocketBy(RocketId fromValue, CancellationToken ct = default);
    
    Task<OperationResult<VoidResult ,string>> Store(RocketAggregate patchedRocket, CancellationToken ct = default);
}
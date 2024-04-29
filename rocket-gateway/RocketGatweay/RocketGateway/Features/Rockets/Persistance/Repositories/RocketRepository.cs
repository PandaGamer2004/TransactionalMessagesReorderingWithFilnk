using RocketGateway.Features.Rockets.Core.Aggregates;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Repositories;
using RocketGateway.Features.Rockets.Core.Repositories.Models;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Persistance.Repositories;

public class RocketRepository: IRocketRepository
{
    public Task<OperationResult<VoidResult, string>> Store(RocketAggregate patchedRocket, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<RocketAggregate>, string>> Query(QuerySpec querySpec)
    {
        throw new NotImplementedException();
    }

    Task<OperationResult<RocketAggregate, string>> IRocketRepository.LoadRocketBy(RocketId fromValue, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
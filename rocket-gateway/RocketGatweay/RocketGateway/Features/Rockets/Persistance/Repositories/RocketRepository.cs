using Microsoft.EntityFrameworkCore;
using RocketGateway.DbContext;
using RocketGateway.DbContext.Models;
using RocketGateway.Features.Rockets.Core.Aggregates;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Repositories;
using RocketGateway.Features.Rockets.Core.State;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;

namespace RocketGateway.Features.Rockets.Persistance.Repositories;

public class RocketRepository: IRocketRepository
{
    private readonly RocketsDbContext rocketsDbContext;
    private readonly ILogger<RocketRepository> logger;

    public RocketRepository(
        RocketsDbContext rocketsDbContext,
        ILogger<RocketRepository> logger
        )
    {
        this.rocketsDbContext = rocketsDbContext;
        this.logger = logger;
    }
    public Task<OperationResult<VoidResult, string>> Store(RocketAggregate patchedRocket, CancellationToken ct = default)
    {
        return OperationResult<VoidResult, string>.FromThrowableAsyncOperation(async () =>
        {
            var aggregateState = patchedRocket.State;
            RocketDAO rocketDao = new RocketDAO
            {
                Id = aggregateState.Id.Value,
                LaunchSpeed = aggregateState.LaunchSpeed,
                Mission = aggregateState.Mission,
                ExplodedReason = aggregateState.ExplodedReason,
                WasExploded = aggregateState.WasExploded,
                Type = aggregateState.Type
            };
            
            if (await rocketsDbContext.Rockets.AnyAsync(dao => dao.Id == rocketDao.Id, cancellationToken: ct))
            {
                rocketsDbContext.Update(rocketDao);
            }
            else
            {
                await rocketsDbContext.AddAsync(rocketDao, ct);
            }

            await rocketsDbContext.SaveChangesAsync(ct);

            return VoidResult.Instance;
        }, PersistenceErrorProjector("Failed to update aggregate state"));
    }


    private Func<Exception, string> PersistenceErrorProjector(String message)
    {
        return (Exception ex) =>
        {
            logger.LogError(ex, ex.Message, ex.StackTrace);
            return message;
        };
    }
    public Task<OperationResult<IEnumerable<RocketAggregate>, string>> LoadRockets(CancellationToken ct = default)
    {
        return OperationResult<IEnumerable<RocketAggregate>, string>.FromThrowableAsyncOperation(async () =>
        {
            var fetchedRockets 
                = await rocketsDbContext.Rockets
                    .ToListAsync(cancellationToken: ct);
            return fetchedRockets
                .Select(AggregateStateFromDao)
                .Select(RocketAggregate.FromExternalState);
        }, PersistenceErrorProjector("Failed to load rockets"));
    }

    Task<OperationResult<RocketAggregate, string>> IRocketRepository.LoadRocketBy(RocketId fromValue,
        CancellationToken ct)
    {
        return OperationResult<RocketAggregate, string>.FromThrowableAsyncOperation(async () =>
        {
           var targetDao = await rocketsDbContext.Rockets.Where(
               it => it.Id == fromValue.Value
               ).FirstOrDefaultAsync(cancellationToken: ct);

               if (targetDao == null)
               {
                   return RocketAggregate.FromExternalState(new RocketAggregateState{
                       Id = RocketId.FromValue(Guid.NewGuid()),
                   });
               }

               return RocketAggregate.FromExternalState(new RocketAggregateState
               {
                   Id = RocketId.FromValue(targetDao.Id),
                   ExplodedReason = targetDao.ExplodedReason,
                   WasExploded = targetDao.WasExploded,
                   Type = targetDao.Type,
                   LaunchSpeed = targetDao.LaunchSpeed,
                   Mission = targetDao.Mission
               });
        }, err => "Failed to load aggregate state");
    }

    private RocketAggregateState AggregateStateFromDao(RocketDAO targetDao)
        => new RocketAggregateState
        {
            Id = RocketId.FromValue(targetDao.Id),
            ExplodedReason = targetDao.ExplodedReason,
            WasExploded = targetDao.WasExploded,
            Type = targetDao.Type,
            LaunchSpeed = targetDao.LaunchSpeed,
            Mission = targetDao.Mission
        };
}
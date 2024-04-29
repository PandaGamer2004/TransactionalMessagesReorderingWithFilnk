using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Repositories;
using RocketGateway.Features.Rockets.Core.Services.Instances;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Core.State;
using RocketGateway.Features.Rockets.Core.Validation;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Rockets.Framework.Mapping;
using RocketGateway.Features.Rockets.Framework.Messaging.Producers;
using RocketGateway.Features.Rockets.Persistance.Repositories;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Validation;

namespace RocketGateway.Extensions;

public static class DIExtensions
{
    public static IServiceCollection RegisterDI(this IServiceCollection serviceCollection)
    {
        
        serviceCollection.AddScoped<IRocketChangeEventProducer, RocketChangeEventProducer>();
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        serviceCollection.AddControllers();


        serviceCollection.AddScoped<
            IRocketRepository, 
            RocketRepository
        >();

        serviceCollection
            .AddScoped<IMapper<BatchedModificationEvent, OperationResult<BatchedCoreModificationEvent, ErrorModel>>,
                BatchCireModelModificationEventMapper>();


        serviceCollection.AddScoped<
            IMapper<RocketAggregateState, ResponseRocketModel>,
            RocketModelMapper
        >();

        serviceCollection.AddScoped<IRocketsService, RocketsService>();

        serviceCollection
            .AddSingleton<IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>>,
                RocketChangeEventMapper>();

        serviceCollection.AddSingleton<
            IValidator<RocketChangeCoreEvent>, 
            RocketUpdateEventValidator
        >();

        return serviceCollection;
    }
}
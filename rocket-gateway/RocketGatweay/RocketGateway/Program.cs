using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Instances;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Core.Validation;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Rockets.Framework.Mapping;
using RocketGateway.Features.Rockets.Framework.Messaging.Producers;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Validation;

var builder 
    = WebApplication.CreateBuilder(args);




builder.Services.AddScoped<IRocketChangeEventProducer, RocketChangeEventProducer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


builder.Services
    .AddScoped<IMapper<BatchedModificationEvent, OperationResult<BatchedCoreModificationEvent, ErrorModel>>,
        BatchCireModelModificationEventMapper>();
builder.Services.AddScoped<IRocketsService, RocketsService>();
builder.Services
    .AddSingleton<IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>>,
        RocketChangeEventMapper>();
builder.Services.AddSingleton<
    IValidator<RocketChangeCoreEvent>, 
    RocketUpdateEventValidator
>();


builder.AddKafka();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program{}
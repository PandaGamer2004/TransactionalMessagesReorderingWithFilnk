using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Instances;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Core.Validation;
using RocketGateway.Features.Rockets.Mapping;
using RocketGateway.Features.Rockets.Models;
using RocketGateway.Features.Rockets.Models.Events;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Validation;

var builder 
    = WebApplication.CreateBuilder(args);




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IRocketsService, RocketsService>();
builder.Services
    .AddSingleton<IMapper<RocketChangeEvent, OperationResult<RocketChangeDomainEvent, ErrorModel>>,
        RocketChangeEventMapper>();
builder.Services.AddSingleton<
    IValidator<RocketChangeDomainEvent>, 
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
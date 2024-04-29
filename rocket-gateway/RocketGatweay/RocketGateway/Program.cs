using RocketGateway.Extensions;

var builder 
    = WebApplication.CreateBuilder(args);



builder.Services.RegisterDI();
builder.AddPostgres();
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
using Common.SharedKernel.Behaviours;
using Module.Orders;
using Modules.Warehouse;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Common MediatR behaviors across all modules
builder.Services.AddMediatR(config =>
{
    config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
});

builder.Services.AddOrders();
builder.Services.AddWarehouse(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOrders();
await app.UseWarehouse();

app.Run();

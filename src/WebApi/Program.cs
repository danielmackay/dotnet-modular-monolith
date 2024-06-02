using Common.SharedKernel.Behaviours;
using Modules.Orders.Endpoints;
using Modules.Warehouse.Endpoints;

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



builder.Services.AddOrdersServices();
builder.Services.AddWarehouseServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOrdersModule();
await app.UseWarehouseModule();

app.Run();

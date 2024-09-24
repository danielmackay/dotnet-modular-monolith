using Common.SharedKernel;
using Modules.Catalog;
using Modules.Orders;
using Modules.Warehouse;
using Scalar.AspNetCore;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddSwagger();

    builder.Services.AddGlobalErrorHandler();

    builder.Services.AddCommon();

    builder.Services.AddMediatR();

    // builder.Services.AddOrders();
    builder.Services.AddWarehouse(builder.Configuration);
    builder.Services.AddCatalog(builder.Configuration);
}

var app = builder.Build();
{
    app.MapOpenApi();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // app.UseSwagger();
        // app.UseSwaggerUI();
        // app.MapScalarUi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.UseOrders();
    app.UseWarehouse();
    app.UseCatalog();

    app.Run();
}
using Common.SharedKernel;
using Modules.Catalog;
using Modules.Customers;
using Modules.Orders;
using Modules.Warehouse;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    // Add service defaults & Aspire components.
    builder.AddServiceDefaults();

    builder.Services.AddSwagger();

    builder.Services.AddGlobalErrorHandler();

    builder.Services.AddCommon();

    builder.Services.AddMediatR();

    // builder.Services.AddOrders();
    builder.AddWarehouse();
    builder.AddCatalog();
    builder.AddCustomers();
    builder.AddOrders();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseOrders();
    app.UseWarehouse();
    app.UseCatalog();
    app.UseCustomers();

    app.MapDefaultEndpoints();

    app.Run();
}
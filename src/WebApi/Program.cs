using Common.SharedKernel;
using Modules.Catalog;
using Modules.Customers;
using Modules.Orders;
using Modules.Warehouse;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddSwagger();

    builder.Services.AddGlobalErrorHandler();

    builder.Services.AddCommon();

    builder.Services.AddMediatR();

    builder.Services.AddWarehouse(builder.Configuration);
    builder.Services.AddCatalog(builder.Configuration);
    builder.Services.AddCustomers(builder.Configuration);
    builder.Services.AddOrders(builder.Configuration);
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

    app.Run();
}

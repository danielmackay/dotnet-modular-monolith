# dotnet-modular-monolith

[![.NET](https://github.com/danielmackay/dotnet-modular-monolith/actions/workflows/dotnet.yml/badge.svg)](https://github.com/danielmackay/dotnet-modular-monolith/actions/workflows/dotnet.yml)

## Goal

Sample repo demoing how an e-commerce system can be architected as a modular monolith using ASP.NET Core.

## Technology

- .NET 9.0
- Aspire
- ASP.NET Core Minimal API
- Entity Framework Core
- FluentValidation
- TestContainers
- MediatR
- Bogus
- ErrorOr
- Open Telemetry
- Swagger

## Architecture / Patterns

- Modular Monolith
- Domain Driven Design
- VSA (within each module)
- REPR Pattern
- CQRS
- Strongly Typed IDs
- Result Pattern
- API Discovery

## Modules

### Orders Module

Responsible for Order Management

- Order Creation
- Order Lifecycle Management
- Payment: taking and recording payment
- Shipping: Calculating fee, dispatching, tracking

### Warehouse Module

Responsible for Warehouse and Inventory Management

- Stock Levels - How much do we actually have?
- Inventory Tracking - Where is the item in the warehouse?
- Supplier Chain - Where can I buy the item from?
- Restocking - When do we need to restock?
- Backorders - When do we put items on back order?
- Product Management

## Business Invariants

Warehouse:
- ✅ An Aisle cannot have duplicate bays
- ✅ A bay cannot have duplicate shelves

Product Catalog:
- ✅ A product must have a name
- ✅ A product can be given one or more categories
- ✅ A product cannot have a negative price
- ✅ A product cannot duplicate categories

Customers:
- Must have a unique email address
- Must have an address

Cart:
- ✅ Must always have the correct price

Orders:
- ✅ An order must be associated with a customer
- ✅ The order total must always be correct
- ✅ The order tax must always be correct
- ✅ Shipping must be included in the total price
- ✅ Payment must be completed for the order to be placed (FUTURE: Consider splitting payments to its own module)

## Event Storming

![image](https://github.com/user-attachments/assets/63ceadd7-428e-4cac-9937-377e46ae384a)

## Bounded Contexts

### Warehouse Context

![image](https://github.com/user-attachments/assets/f7d5a522-246b-4ecf-88cf-e5cb0738f0a0)

### Catalog Context

![image](https://github.com/user-attachments/assets/a08d2964-c3fa-417f-8b6a-598d2a2fe511)

### Customer Context

![image](https://github.com/user-attachments/assets/d104aff7-2d5d-4308-af97-fca0c298d0b7)

### Orders Context

![image](https://github.com/user-attachments/assets/3c731981-1f98-42ca-9f74-c955a31a5790)

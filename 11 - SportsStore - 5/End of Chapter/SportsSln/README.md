# SportsStore - Distributed Order Processing Platform
**Student:** Tiago Borges 73638  
**Module:** Full Stack Development - Assignment 2  
**Institution:** Dorset College  

## System Architecture

This system is a distributed event-driven order processing platform built with .NET 8, RabbitMQ, Blazor, and React.

### Architecture Overview
## Projects

| Project | Technology | Port | Description |
|---------|-----------|------|-------------|
| SportsStore.OrderApi | .NET 8 Web API | 5049 | Central order management API |
| SportsStore.InventoryService | .NET 8 Worker | - | Validates stock availability |
| SportsStore.PaymentService | .NET 8 Worker | - | Processes payments |
| SportsStore.ShippingService | .NET 8 Worker | - | Creates shipments |
| SportsStore.CustomerPortal | Blazor Server | 5178 | Customer-facing UI |
| AdminDashboard | React + TypeScript | 3000 | Admin operations UI |
| SportsStore.Shared | .NET 8 Class Library | - | Shared events and models |

## Event Flow

1. Customer checks out via Blazor portal
2. OrderApi creates order and publishes to RabbitMQ queue: inventory-check
3. InventoryService consumes event, validates stock, publishes to: payment-process
4. PaymentService processes payment, publishes to: shipping-create
5. ShippingService creates shipment with tracking reference

## Order States

Cart ? Submitted ? InventoryPending ? InventoryConfirmed/Failed ? PaymentPending ? PaymentApproved/Failed ? ShippingPending ? ShippingCreated ? Completed/Failed

## How to Run

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- Node.js v20+

### Option 1 - Docker Compose (Recommended)
`ash
cd "11 - SportsStore - 5/End of Chapter/SportsSln"
docker-compose up --build
`

### Option 2 - Manual
1. Start RabbitMQ:
`ash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
`

2. Start OrderApi:
`ash
cd SportsStore.OrderApi
dotnet run
`

3. Start InventoryService:
`ash
cd SportsStore.InventoryService
dotnet run
`

4. Start PaymentService:
`ash
cd SportsStore.PaymentService
dotnet run
`

5. Start ShippingService:
`ash
cd SportsStore.ShippingService
dotnet run
`

6. Start Blazor CustomerPortal:
`ash
cd SportsStore.CustomerPortal
dotnet run
`

7. Start React Admin Dashboard:
`ash
cd sportsstore-admin
npm install
npm start
`

## Service Responsibilities

- **OrderApi** - Creates orders, stores in SQL Server, publishes events to RabbitMQ, exposes REST endpoints
- **InventoryService** - Consumes inventory-check queue, simulates stock validation (90% success rate)
- **PaymentService** - Consumes payment-process queue, simulates payment (85% success rate)
- **ShippingService** - Consumes shipping-create queue, generates tracking reference
- **CustomerPortal** - Browse products, add to cart, checkout, view orders
- **AdminDashboard** - View all orders, filter by status, identify failed orders

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/orders/checkout | Submit new order |
| GET | /api/orders | Get all orders |
| GET | /api/orders/{id} | Get order by ID |
| GET | /api/orders/{id}/status | Get order status |
| PATCH | /api/orders/{id}/status | Update order status |

## Technologies Used

- .NET 8 - Backend services
- RabbitMQ - Message broker
- Entity Framework Core - ORM
- SQL Server LocalDB - Database
- Serilog - Structured logging
- Blazor Server - Customer portal
- React + TypeScript - Admin dashboard
- Docker + Docker Compose - Containerisation
- Swagger/OpenAPI - API documentation

## Logging

All services use Serilog structured logging with the following sinks:
- Console output
- File (daily rolling logs in /logs folder)

Key events logged: order submission, message publishing, message consumption, inventory validation, payment outcome, shipping creation, errors.

## Assumptions and Limitations

- Inventory validation is simulated with 90% success rate
- Payment processing is simulated with 85% success rate
- Products are currently hardcoded in the CustomerPortal
- SQLite can be used as alternative to SQL Server
- RabbitMQ must be running before starting any service

# SportsStore - Distributed Order Processing Platform

Student: Tiago Borges | Student ID: 73638
Module: Full Stack Development - Semester 2, Assignment 2
Programme: BSc in Computing | Institution: Dorset College

## What is this project?

This project extends the SportsStore shopping cart application into a distributed order processing system. When a customer places an order, instead of processing everything in one go, the system breaks it down into smaller services that communicate with each other using messages through RabbitMQ.

The idea is to simulate how a real e-commerce platform works behind the scenes - when you buy something online, there are separate systems checking stock, processing your payment, and arranging delivery. That is exactly what this project does.

## How the system works

When a customer checks out, here is what happens step by step:

1. The customer fills their cart in the Blazor Customer Portal and clicks checkout
2. The Order API receives the order, saves it to the database, and sends a message to RabbitMQ
3. The Inventory Service picks up the message and checks if the items are in stock
4. If stock is confirmed, the Payment Service processes the payment
5. If payment is approved, the Shipping Service creates a shipment with a tracking reference
6. Throughout this process, the order status is updated at each step

## Projects in this solution

- SportsStore.OrderApi: The main API - handles orders, talks to the database, and publishes messages to RabbitMQ
- SportsStore.InventoryService: Listens for new orders and checks if items are in stock
- SportsStore.PaymentService: Processes payments when inventory is confirmed
- SportsStore.ShippingService: Creates shipment details when payment goes through
- SportsStore.CustomerPortal: Blazor web app where customers browse products and place orders
- AdminDashboard: React app where admins can monitor all orders and spot failures
- SportsStore.Shared: Shared library containing the message contracts used between services

## How to run the project

Prerequisites: .NET 8 SDK, Docker Desktop, Node.js

Start RabbitMQ:
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

Start OrderApi:
cd SportsStore.OrderApi then dotnet run

Start each service in separate terminals:
cd SportsStore.InventoryService then dotnet run
cd SportsStore.PaymentService then dotnet run
cd SportsStore.ShippingService then dotnet run

Start Customer Portal:
cd SportsStore.CustomerPortal then dotnet run

Start Admin Dashboard:
cd sportsstore-admin then npm install then npm start

## Assumptions and Limitations

- Inventory validation is simulated with a 90% success rate
- Payment processing is simulated with an 85% success rate
- Products are currently hardcoded in the Customer Portal
- RabbitMQ must be running before starting any service
- The database is created automatically when the Order API starts for the first time

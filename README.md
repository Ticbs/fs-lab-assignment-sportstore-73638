# SportsStore Modernisation Project

Student: Tiago da Cunha Borges 73638 
Module: Full Stack Development  
Programme: BSc in Computing  
Institution: Dorset College  

---

# Project Overview

This project modernises and extends the **SportsStore application** originally developed using **ASP.NET Core .NET 6** from the book *Pro ASP.NET Core*.

The objective of this assignment is to demonstrate professional software engineering practices including:

• Framework upgrade  
• Structured logging  
• Third-party payment integration  
• CI pipeline implementation  
• Secure configuration management  

The application was upgraded and extended using modern development practices.

---

# Technologies Used

| Technology | Purpose |
|-------------|--------|
ASP.NET Core (.NET 8) | Web application framework |
Serilog | Structured logging |
Stripe .NET SDK | Payment processing |
GitHub Actions | Continuous Integration |
Git | Version control |
User Secrets | Secure configuration |

---

# Part A — Upgrade to .NET 8

The original application targeted **.NET 6**.  
The project was upgraded to **.NET 8, 9** to use the latest stable LTS runtime.

Key changes:

• Updated `TargetFramework` in `.csproj`
• Updated NuGet dependencies
• Verified application builds successfully
• Ensured no breaking changes affected functionality

Example:

```xml
<TargetFramework>net8.0</TargetFramework>

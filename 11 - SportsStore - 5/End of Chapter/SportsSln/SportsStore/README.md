
# SportsStore - Full Stack Assignment

## Overview
Upgraded Sports Store application from .NET 6 to .NET 8 with structured logging (Serilog) and Stripe payment integration.

**Student:** Tiago Borges (73638)  
**Module:** Full Stack Development  
**Assignment Weight:** 30% of Module Grade

---

## Requirements Implemented

### Part A: .NET Upgrade ✅
- Upgraded from .NET 6 to .NET 8
- Updated all NuGet dependencies
- Resolved breaking changes
- Clean build with no warnings

### Part B: Serilog Integration ✅
- Structured logging via Serilog.AspNetCore
- Console and rolling file sinks
- Enrichment with machine name and environment
- Configured via appsettings.json
- Logs: startup, checkout/payment, exceptions, order creation

### Part C: Stripe Integration ✅
- Official Stripe .NET SDK integrated
- Secure checkout workflow
- Test keys via environment variables/user secrets
- Payment validation before order confirmation
- Handles: success, failure, cancellation

### Part D: GitHub Actions CI ✅
- Workflow triggers on push/PR to main
- Builds, restores dependencies, runs tests
- Fails if tests fail

### Part E: Professional Practice ✅
- Branch naming: `tb-feature-description`
- Commit messages include: "Tiago Borges 73638"
- Regular commits every 48 hours

---

## Local Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Stripe account (test mode)

### Installation

1. **Clone repository:**
2. **Configure user secrets for Stripe:**
3. **Update database:**
4. **Run application:**
Navigate to `https://localhost:5001`

---

## Logging Configuration

### Output Destinations
- **Console:** Real-time logs during development
- **File:** Daily rolling logs in `logs/` directory (30-day retention)

### Log Levels
- Development: Debug (verbose)
- Production: Information (minimal)

### Events Logged
---

## Stripe Integration

### Configuration

**appsettings.json** (committed):

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_51MO1K1LJKGz123456789",
    "SecretKey": "sk_test_51MO1K1LJKGz123456789"
  }
}
```

**User Secrets** (NOT committed):

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_51MO1K1LJKGz987654321",
    "SecretKey": "sk_test_51MO1K1LJKGz987654321"
  }
}
```

### Test Cards
- Success: `4242 4242 4242 4242`
- Decline: `4000 0000 0000 0002`
- Expired: `4000 0000 0000 0069`

### Payment Flow
1. User clicks "Checkout"
2. Stripe payment form displayed
3. Card validated
4. Payment intent created
5. Order created only after successful payment
6. Confirmation email sent (if configured)

---

## CI/CD Pipeline

GitHub Actions automatically:
- Restores NuGet packages
- Builds in Release mode
- Runs all unit tests
- Reports failures

View pipeline status: [Actions Tab](https://github.com/Ticbs/fs-lab-assignment-sportstore-73638/actions)

---

## Project Structure

---

## License
Academic assignment - Full Stack Development Module

---

**Last Updated:** March 2026  
**Status:** ✅ Complete
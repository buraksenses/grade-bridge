# ADR 0001 - Use Clean Architecture

## Status

Accepted

## Context

GradeBridge will grow beyond a single script or single institution integration. The system needs clear separation between API layer, application logic, domain model, infrastructure concerns, and external integrations.

## Decision

Use a Clean Architecture-inspired structure:

```text
GradeBridge.Api
GradeBridge.Application
GradeBridge.Domain
GradeBridge.Infrastructure
```

## Consequences

Positive: business logic is separated from framework details, testing becomes easier, external adapters are isolated, future integrations can be added cleanly.

Negative: more initial structure and slightly slower first development.

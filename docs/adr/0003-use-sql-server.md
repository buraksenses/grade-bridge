# ADR 0003 - Use SQL Server

## Status

Accepted for MVP

## Context

The project is being developed with .NET. The team is comfortable with SQL Server. The domain requires relational consistency.

## Decision

Use SQL Server for the MVP. Local development uses SQL Server in Docker. Production may use Azure SQL.

## Consequences

Positive: strong .NET compatibility, familiar tooling, good fit for relational data.

Negative: PostgreSQL may be cheaper in some environments; cloud SQL cost must be monitored.

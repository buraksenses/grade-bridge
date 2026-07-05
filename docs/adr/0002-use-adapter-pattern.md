# ADR 0002 - Use Adapter Pattern

## Status

Accepted

## Context

GradeBridge must support different file formats, optical reader outputs, university systems, API formats, and export formats.

## Decision

Use adapters for external inputs and outputs. Adapter types: file parser adapters, export adapters, transfer adapters.

## Consequences

Positive: core remains stable, new institutions can be added by adding adapters, testing each integration is easier.

Negative: requires careful interface design and adapter configuration.

---
name: vertical-slice-development
description: "Creates and tests one-file .NET Minimal API vertical slices with IEndpoint and FluentValidation - Brought to you by frdiana/verticalslice.template"
argument-hint: "resource=... operation=... route=..."
user-invocable: true
disable-model-invocation: false
---

# Vertical Slice Development

## Overview

Add API operations using the architecture already present in the generated
solution. Use this skill for endpoint creation, request validation, typed HTTP
results, OpenAPI metadata, Domain ports, Infrastructure implementations, and
focused tests.

## Prerequisites

* A generated Vertical Slice solution
* A clear resource, HTTP operation, route, and expected outcomes
* Existing project instructions and neighboring endpoint patterns

## Quick Start

1. Inspect `Endpoints/Examples/Echo.cs` and its tests.
2. Create `Endpoints/<Resource>/<Operation>.cs` implementing `IEndpoint`.
3. Keep transport request, response, handler, and validator in that file.
4. Add Domain and Infrastructure code only when the operation requires business
   behavior or an external dependency.
5. Add validator unit tests and HTTP integration tests.

Preserve the generated authentication and persistence choices. Entra endpoints
use the existing configured authorization extension. Persistence code implements
domain-specific contracts and remains inside Infrastructure.

## Endpoint Contract

Every endpoint must:

* Map exactly one HTTP operation
* Use nested sealed records for transport contracts
* Use a nested sealed `RequestValidator` for constrained input
* Depend on Domain interfaces rather than Infrastructure types
* Return `TypedResults`
* Use explicit `Results<T1, T2>` signatures when multiple outcomes exist
* Add name, summary, description, tags, and all response metadata
* Propagate `CancellationToken` through asynchronous calls
* Avoid exposing persistence entities

Use the endpoint filter already registered by the API:

```csharp
.AddEndpointFilter<ValidationFilter<Request>>()
```

Do not add validation filters to request types without a registered validator.

## HTTP Semantics

| Operation | Success response                         |
|-----------|------------------------------------------|
| Get one   | `200 OK` or `404 Not Found`              |
| List      | `200 OK`                                 |
| Create    | `201 Created` with a location header     |
| Update    | `200 OK` or `204 No Content`              |
| Delete    | `204 No Content`                          |

Represent validation as `400`, missing resources as `404`, and state conflicts
as `409`. Keep documented response metadata aligned with the handler signature.

## Project Boundaries

1. Put business types and ports in Domain.
2. Put database, queue, filesystem, and external HTTP implementations in
   Infrastructure.
3. Register implementations in `Infrastructure/DependencyInjection.cs`.
4. Keep API code limited to transport mapping and orchestration.
5. Add AppHost resources only when the Infrastructure implementation requires
   them, then pass references explicitly.

## Testing

Unit test every validator boundary and deterministic Domain rule. Integration
test route binding, serialization, status codes, validation problem keys, and
response metadata visible to clients.

Use `TestContext.Current.CancellationToken` in xUnit asynchronous calls. Do not
leave placeholder tests or assertions that cannot fail meaningfully.

## Validation

```powershell
dotnet build <Solution>.slnx
dotnet test <Solution>.slnx --no-build
```

Use Aspire runtime validation when resource wiring, service discovery, health,
or distributed behavior changes.

## Troubleshooting

* If discovery misses an endpoint, ensure it is a concrete API-assembly class
  implementing `IEndpoint` with a parameterless constructor.
* If validation does not run, confirm the filter generic type exactly matches
  the bound request and the validator is discoverable in the API assembly.
* If typed results fail to infer, declare the handler return type explicitly.

> Brought to you by frdiana/verticalslice.template
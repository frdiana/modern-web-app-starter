---
title: Modern Web App Project Instructions
description: Repository-wide architecture and validation conventions for generated applications
---

## Architecture

Preserve these project boundaries:

* Domain owns business types and interfaces and references no other project
* Infrastructure implements Domain interfaces and owns external integrations
* API is the composition root and HTTP boundary
* AppHost owns distributed application topology only
* ServiceDefaults owns cross-cutting observability, health, resilience, and
  service discovery defaults

Do not introduce `Common`, `Shared`, or `Helpers` projects without a concrete
ownership boundary supported by multiple real consumers.

## Minimal API Vertical Slices

Use one endpoint class per HTTP operation under
`src/backend/<Solution>.Api/Endpoints/<Resource>/`.

Each endpoint file owns:

* Route mapping through `IEndpoint`
* Transport request and response records
* Handler
* FluentValidation validator when input has constraints
* OpenAPI name, summary, description, tags, and response metadata

Use typed results. Accept and propagate `CancellationToken` through all async
calls. Inject Domain interfaces or application services, never persistence
clients or concrete repositories into endpoint handlers.

Return expected validation, not-found, and conflict outcomes as typed results.
Reserve exceptions for exceptional failures and let the global exception handler
produce safe RFC 7807 responses.

## Authentication And Persistence

Preserve the authentication mode generated into the solution. When Entra is
present, use MSAL on the frontend, JWT bearer validation on the API, and the
existing delegated-scope policy. Never add a client secret to the SPA.

Use domain-specific repository contracts. Do not replace them with a generic
repository abstraction. Keep Cosmos SDK, Dapper, SQL client, and Npgsql types in
Infrastructure. Add or modify Aspire resources only in AppHost.

Integration tests replace external repositories with deterministic test doubles.
Use Aspire runtime tests when provider behavior or resource wiring changes.

## Development

Use central package management in `Directory.Packages.props`. Add compatible
stable package versions and retain NuGet vulnerability checks.

Run focused tests after each change, then run:

```powershell
dotnet build <Solution>.slnx
dotnet test <Solution>.slnx --no-build
```

Operate AppHost with the Aspire CLI. Use `aspire start`, wait for resources with
`aspire wait`, and use `aspire stop` when runtime validation is complete.

The frontend uses Vite, React, and React Router. Keep route components under
`src/frontend/src/routes` and API clients under `src/frontend/src/api`. Browser
requests use relative `/api` paths so Vite can proxy to the Aspire-provided API
endpoint. Run `npm run build` after frontend changes.
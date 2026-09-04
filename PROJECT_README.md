---
title: ModernWebApp
description: Aspire-hosted .NET application using one-file Minimal API vertical slices
---

## Architecture

This solution contains:

* `src/backend/ModernWebApp.Domain` for business types and interfaces
* `src/backend/ModernWebApp.Infrastructure` for external system implementations
* `src/backend/ModernWebApp.Api` for Minimal API vertical slices and composition
* `src/backend/ModernWebApp.AppHost` for the Aspire resource graph
* `src/backend/ModernWebApp.ServiceDefaults` for observability, health checks, resilience,
  and service discovery
* Unit and HTTP integration test projects under `src/backend/tests`
* Vite and React frontend with Home and About routes
* Aspire-managed API endpoint injection for the Vite development proxy
* Authentication and persistence selected when the solution was generated
* Structured logging, validated configuration, feature flags, and rate limiting

## Build And Test

```powershell
dotnet build ModernWebApp.slnx
dotnet test ModernWebApp.slnx --no-build
cd src\frontend
npm install
npm run build
```

## Develop With The Light Spec Workflow

Use `Modern Web App Architect` as the single user-facing Copilot agent. It turns
project ideas into a project spec and roadmap, prepares feature specs for
approval, dispatches implementation to backend and React specialists, and runs
an independent assessment before marking work validated.

```text
/project-discover idea=...
/feature-spec featureId=next feature=...
/feature-implement featureId=F-001
/feature-assess featureId=F-001
/roadmap-status
```

The versioned artifacts under `.specs` show what is planned, implemented,
validated, blocked, or deferred. Read the
[agentic development workflow](docs/development-workflow.md) for all agents,
prompts, status transitions, and a complete example.

## Run

```powershell
aspire start --apphost src\backend\ModernWebApp.AppHost\ModernWebApp.AppHost.csproj
```

Wait for the frontend before using browser automation:

```powershell
aspire wait frontend --non-interactive
```

The Home route calls this example endpoint through the Vite proxy:

```http
GET /api/examples/echo?message=Hello+World
```

The persistence example uses:

```http
POST /api/examples/greetings
GET /api/examples/greetings
```

## Configuration

`Application`, `RateLimiting`, and `FeatureManagement` settings live in API
configuration. Invalid required options fail during startup.

`ReverseProxy:KnownProxies` controls which proxies may supply forwarded client
IP and protocol headers. Add deployment ingress addresses explicitly so the
rate limiter partitions anonymous callers correctly.

When Entra files are present, copy `src/frontend/.env.example` to
`src/frontend/.env.local` and replace the tenant, SPA client, API client, and
scope values. Configure matching API settings through user secrets. MSAL uses
Authorization Code Flow with PKCE and adds access tokens to API requests.
Register `http://localhost:5173` as the SPA redirect URI and start Entra projects
without `--isolated` so the registered URI remains stable.

When an external persistence provider is present, AppHost creates and injects
the `greetings` connection string. Docker must be running. The Cosmos client is
limited to the Aspire-provided endpoint so isolated dynamic ports are supported.

## Add A Vertical Slice

Use the `Modern Web App Architect` project agent or invoke `/add-endpoint` in
GitHub Copilot Chat. New API operations belong under
`src/backend/ModernWebApp.Api/Endpoints/<Resource>` with one operation per file.

Replace the `Examples/Echo` slice when the first real capability is added.

## Extend The Frontend

Add routes under `src/frontend/src/routes` and API clients under
`src/frontend/src/api`. Keep browser requests relative to `/api`; AppHost
provides the backend endpoint to the Vite proxy without fixed localhost ports.
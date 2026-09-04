---
title: Modern Web App Starter
description: Local dotnet new template for an Aspire-hosted vertical slice application
---

## Purpose

This repository is a local `dotnet new` template for starting a .NET 10
application with Aspire and a minimal React frontend.

The generated solution includes:

* ASP.NET Core Minimal API with one operation per endpoint file
* `IEndpoint` discovery and deterministic registration
* Request, response, handler, and FluentValidation validator in one vertical
  slice
* Typed results, OpenAPI metadata, and RFC 7807 problem details
* Domain and Infrastructure project boundaries
* Aspire AppHost and ServiceDefaults
* Unit and HTTP integration tests
* Project-local GitHub Copilot agent, instructions, and skill
* Backend projects and tests grouped under `src/backend`
* Vite and React frontend with client-side routing
* Home page that calls the example API through the Aspire-provided endpoint
* Optional Microsoft Entra ID authentication with MSAL and PKCE
* Optional Cosmos DB, SQL Server, or PostgreSQL persistence
* Structured request logging, validated configuration, feature flags, and rate
  limiting

## Install Locally

Clone the repository to a stable local path. The installation records this path,
so do not delete it while the template is installed.

```powershell
git clone https://github.com/frdiana/modern-web-app-starter.git C:\templates\modern-web-app-starter
dotnet new install C:\templates\modern-web-app-starter
```

When working directly from this repository, install the current folder:

```powershell
dotnet new install .
```

No NuGet publication is required.

## Create A Project

Choose a valid C# root namespace. Dotted names are supported.

```powershell
dotnet new modern-web-app `
  --name Acme.Orders `
  --output C:\source\Acme.Orders `
  --auth none `
  --persistence none
```

The command replaces `ModernWebApp` in solution names, project names,
namespaces, project references, and documentation.

## Template Options

| Option          | Values                                    | Default |
|-----------------|-------------------------------------------|---------|
| `--auth`        | `none`, `entra`                           | `none`  |
| `--persistence` | `none`, `cosmos`, `sqlserver`, `postgres` | `none`  |

Examples:

```powershell
dotnet new modern-web-app -n Acme.Secure --auth entra
dotnet new modern-web-app -n Acme.Documents --persistence cosmos
dotnet new modern-web-app -n Acme.Orders --auth entra --persistence postgres
```

The `none` persistence option uses a process-local in-memory repository. Other
options add only their selected driver, repository implementation, and Aspire
resource integration.

## Run The Generated Project

```powershell
cd C:\source\Acme.Orders
dotnet build Acme.Orders.slnx
dotnet test Acme.Orders.slnx
cd src\frontend
npm install
npm run build
cd ..\..
aspire start --apphost src\backend\Acme.Orders.AppHost\Acme.Orders.AppHost.csproj
```

Use `aspire wait frontend --non-interactive` before browser automation. The Home
route calls `GET /api/examples/echo?message=Hello+World` through the Vite proxy.
The About route demonstrates client-side navigation.

## Entra Setup

The `entra` option uses Authorization Code Flow with PKCE through MSAL. Create
two Entra app registrations:

1. Register an API application, expose `access_as_user`, and copy its client ID.
2. Register a SPA application and add `http://localhost:5173` as a SPA redirect
  URI.
3. Grant the SPA delegated permission to the API scope.
4. Copy `src/frontend/.env.example` to `src/frontend/.env.local` and replace its
   placeholders.
5. Configure matching API identifiers through user secrets.

```powershell
dotnet user-secrets set "Entra:TenantId" "<tenant-id>" --project src\backend\Acme.Secure.Api
dotnet user-secrets set "Entra:ClientId" "<api-client-id>" --project src\backend\Acme.Secure.Api
dotnet user-secrets set "Entra:Scope" "access_as_user" --project src\backend\Acme.Secure.Api
```

The SPA contains no client secret. MSAL stores its token cache in session
storage and acquires API access tokens silently when possible.

Start Entra projects without `--isolated`. Their frontend endpoint is pinned to
port `5173` because Entra redirect URIs must be registered in advance. The
default `none` authentication mode keeps dynamically allocated frontend ports.

## Persistence Examples

All providers implement the domain-specific `IGreetingRepository` contract.
The `POST /api/examples/greetings` and `GET /api/examples/greetings` operations
exercise the selected implementation.

* Cosmos creates a `greetings` database and container through the preview local
  emulator. The SDK is limited to the Aspire-provided endpoint so isolated
  dynamic ports remain supported.
* SQL Server uses Dapper and `Microsoft.Data.SqlClient`, with idempotent table
  creation.
* PostgreSQL uses Dapper and Npgsql, with idempotent table creation.

Docker must be running for external persistence options.

## Always-On Examples

The generated API includes:

* Source-generated structured request logs with method, path, status, elapsed
  time, and scoped trace ID
* `IValidateOptions` for application and rate-limit settings, plus provider
  configuration where applicable
* `Microsoft.FeatureManagement` with the `GreetingEndpoint` flag
* A fixed-window `api` rate-limit policy returning `429`

Tests demonstrate invalid configuration, disabled features, rate-limit
rejection, authorization, and repository behavior.

The local Vite proxy forwards client IP and protocol headers. Before deploying
behind another ingress, add only its trusted IP addresses to
`ReverseProxy:KnownProxies`; untrusted forwarded headers are ignored.

## Update The Installed Template

```powershell
git -C C:\templates\modern-web-app-starter pull
dotnet new install C:\templates\modern-web-app-starter --force
```

Updating the installed template affects only projects created afterward.

## Uninstall

Use the same source path used during installation:

```powershell
dotnet new uninstall C:\templates\modern-web-app-starter
```

## Maintain The Template

Keep `ModernWebApp` as the source token in code, file names, and the template
manifest. Test every change by installing from this folder and generating a
project with an unrelated dotted name.

Do not use GitHub's **Use this template** action when automatic renaming is
required. Clone and install with `dotnet new` instead.
---
title: Feature Spec Template
description: Template for planning, implementing, and validating one feature
---

## Feature

ID: `<F-001>`

Name: `<feature name>`

Status: `Planned`

Created: `<yyyy-mm-dd>`

Updated: `<yyyy-mm-dd>`

## User Goal

* `<What the user wants to accomplish>`

## Scope

* `<In-scope behavior>`

## Non-Goals

* `<Out-of-scope behavior>`

## Acceptance Criteria

* [ ] `<Observable outcome>`
* [ ] `<Error, empty, authorization, or accessibility outcome>`
* [ ] `<Required tests and validation pass>`

## Dependencies

* `<Feature ID, external dependency, or none>`

## Assumptions

* `<Assumption and why it is reasonable>`

## Open Questions

* `<Question that blocks or changes implementation, or none>`

## Affected Areas

* Backend/API: `<yes/no and expected slices>`
* Frontend/React: `<yes/no and expected routes or components>`
* Infrastructure/Persistence: `<yes/no and expected adapters>`
* AppHost/Aspire: `<yes/no and expected resources>`

## Implementation Plan

1. `<Small implementation step>`
2. `<Small implementation step>`
3. `<Small implementation step>`

## Test Plan

* Unit tests: `<rules, validation, mapping, or component logic>`
* Integration tests: `<HTTP, auth, persistence, or cross-boundary behavior>`
* Frontend tests: `<component, route, API client, interaction, or accessibility>`

## Validation Commands

* `<Focused test command>`
* `dotnet build ModernWebApp.slnx`
* `dotnet test ModernWebApp.slnx --no-build`
* `npm run build --prefix src/frontend`

## Approval

* Approved by: `<user or pending>`
* Approved on: `<yyyy-mm-dd or pending>`

## Implementation Log

* Production files: `<paths or pending>`
* Tests added or updated: `<paths or pending>`
* Deviations from plan: `<details or none>`

## Validation Evidence

* Acceptance criteria: `<passed, findings, or pending>`
* Commands run: `<commands and results or pending>`
* Validator recommendation: `<Validated, Implemented, Blocked, or pending>`

## Final Result

* Status: `<Implemented, Validated, Blocked, or Deferred>`
* Residual risks: `<known risks or none>`

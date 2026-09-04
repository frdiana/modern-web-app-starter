---
title: Light Spec Kit
description: Lightweight feature specification workflow for Modern Web App agents
---

## Light Spec Kit

Project and feature work starts with a small spec before implementation. The
spec records what will change, why it will change, how it will be tested, and
what remains uncertain.

## Project Discovery

Start with `.specs/project.md` when the work begins from a general application
idea rather than a bounded feature. Use `.specs/project.template.md` to capture
the product shape before implementation starts.

The project spec records:

* Application purpose
* Target users
* Core workflows
* Primary entities
* Roles and authorization expectations
* Technical constraints
* Non-goals
* Glossary
* Product constraints and non-goals

Create `.specs/roadmap.md` alongside the project spec. The roadmap is the compact
progress view and links every feature to its detailed spec.

## Artifact Layout

Create one folder per planned feature:

```text
.specs/
  project.md
  roadmap.md
  features/
    F-001-feature-slug/
      spec.md
```

Use monotonic feature IDs and short lowercase slugs. Never reuse or renumber an
ID after it appears in the roadmap or Git history.

## Status Values

* `Candidate`: The idea exists only in the roadmap
* `Planned`: The feature spec is complete and waiting for approval
* `Approved`: The user approved implementation
* `In Progress`: Specialist agents are implementing the feature
* `Implemented`: Code and tests were written
* `Validated`: Required checks passed
* `Blocked`: Work stopped because a required decision, dependency, or check is
  unavailable
* `Deferred`: The feature remains recorded but is intentionally postponed

The normal lifecycle is:

```text
Candidate -> Planned -> Approved -> In Progress -> Implemented -> Validated
```

## State Ownership

The `Modern Product Spec Architect` creates project, roadmap, and feature specs.
Backend and React specialists implement approved specs and add tests. The
`Modern Feature Validator` assesses evidence without changing files. Only the
`Modern Web App Architect` dispatcher updates implementation and validation
status.

## Required Spec Sections

Each `spec.md` should include:

* Feature name and status
* User goal
* Scope and non-goals
* Acceptance criteria
* Dependencies
* Assumptions and open questions
* Affected areas
* Implementation plan
* Test plan
* Validation commands
* Implementation log
* Validation evidence
* Final result

Keep specs brief. Prefer concrete bullets over long prose.

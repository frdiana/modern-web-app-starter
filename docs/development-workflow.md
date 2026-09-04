---
title: Agentic Development Workflow
description: Light spec workflow, agent catalog, prompt reference, and feature lifecycle
---

## Overview

The generated solution uses one user-facing agent and focused hidden subagents.
The workflow keeps project intent, feature progress, implementation decisions,
tests, and validation evidence versioned beside the code.

Select `Modern Web App Architect` in GitHub Copilot Chat for natural
conversation. The slash prompts provide repeatable entry points for common
operations.

## Workflow

```text
Application idea
  -> project.md and roadmap.md
  -> candidate feature
  -> planned feature spec
  -> user approval
  -> backend and/or React implementation with tests
  -> independent assessment
  -> roadmap and feature spec reconciliation
```

The user speaks only with the dispatcher. Subagents work behind that boundary
and return focused results.

## Spec Artifacts

The light spec-kit uses this structure:

```text
.specs/
  project.md
  roadmap.md
  features/
    F-001-feature-slug/
      spec.md
```

Use [`.specs/project.template.md`](../.specs/project.template.md) for product
purpose, users, workflows, entities, roles, constraints, and glossary. Use
[`.specs/roadmap.template.md`](../.specs/roadmap.template.md) for feature
progress, dependencies, and links. Use
[`.specs/feature.template.md`](../.specs/feature.template.md) for one feature's
acceptance criteria, implementation plan, tests, and validation evidence.

Feature IDs are monotonic. Once `F-001` appears in the roadmap or Git history,
it is never reused or renumbered.

## Feature Status

| Status        | Meaning                                                        |
|---------------|----------------------------------------------------------------|
| `Candidate`   | The idea is recorded in the roadmap                            |
| `Planned`     | The detailed feature spec is ready for approval                |
| `Approved`    | The user approved implementation                               |
| `In Progress` | Specialist agents are implementing production code and tests   |
| `Implemented` | Code and required tests exist and specialist checks pass       |
| `Validated`   | Independent assessment confirms every acceptance criterion     |
| `Blocked`     | A decision, dependency, or required check prevents progress    |
| `Deferred`    | The feature remains recorded but is intentionally postponed    |

The normal path is:

```text
Candidate -> Planned -> Approved -> In Progress -> Implemented -> Validated
```

Only the dispatcher changes implementation and validation states. This keeps
the roadmap, feature specs, and implementation evidence consistent.

## Agent Catalog

| Agent                           | Invocation | Responsibility                                      |
|---------------------------------|------------|-----------------------------------------------------|
| `Modern Web App Architect`      | User       | Dispatch, approvals, state, integration, reporting  |
| `Modern Product Spec Architect` | Subagent   | Project discovery, roadmap, feature specifications  |
| `Modern Backend API Architect`  | Subagent   | .NET API, Domain, Infrastructure, backend tests     |
| `Modern React App Architect`    | Subagent   | React UI, reusable components, API clients, tests   |
| `Modern Feature Validator`      | Subagent   | Read-only assessment against specs and test results |

### Modern Web App Architect

The dispatcher is the only agent users need to select. It classifies requests,
invokes the right specialists, owns status transitions, reconciles artifacts,
and reports the next action.

### Modern Product Spec Architect

The product specialist writes only under `.specs`. It uses assumption-first
discovery, asks at most three blocking questions per round, allocates stable
feature IDs, and prepares implementation-ready specs. It never writes production
code or approves features.

### Modern Backend API Architect

The backend specialist implements Minimal API vertical slices while preserving
Domain, Infrastructure, API, AppHost, and ServiceDefaults boundaries. Every code
change includes relevant unit tests, integration tests, or both.

### Modern React App Architect

The frontend specialist uses React 19+, Vite, React Router, and approachable
TypeScript. It favors clear reusable components without advanced type machinery
and includes tests for component behavior, routes, API clients, interactions,
and applicable UI states.

### Modern Feature Validator

The validator independently maps acceptance criteria to code, tests, and
executable checks. It does not modify files or fix findings. It recommends a
status to the dispatcher based on evidence.

## Prompt Reference

| Prompt               | Purpose                                                   |
|----------------------|-----------------------------------------------------------|
| `/project-discover`  | Turn an application idea into `project.md` and a roadmap  |
| `/feature-spec`      | Create or refine one planned feature spec                 |
| `/feature-implement` | Approve, implement, test, and assess a planned feature    |
| `/feature-assess`    | Reassess existing implementation without fixing findings |
| `/roadmap-status`    | Reconcile and summarize progress without changing code    |
| `/add-endpoint`      | Start a bounded API endpoint through the same workflow    |

### Discover A Project

```text
/project-discover idea="A mobile-first site that generates games for adults to play with children"
```

The dispatcher creates or updates `.specs/project.md` and
`.specs/roadmap.md`. It does not implement code during discovery.

### Specify A Feature

```text
/feature-spec featureId=next feature="Collect activity preferences"
```

The product specialist allocates the next ID, writes the feature spec, changes
the roadmap entry to `Planned`, and stops for approval.

### Implement A Feature

```text
/feature-implement featureId=F-001
```

This command is explicit approval when no blocking questions remain. The
dispatcher changes the status, delegates implementation, requires tests, invokes
independent assessment, and reconciles the final status.

### Assess A Feature

```text
/feature-assess featureId=F-001
```

The validator runs checks and returns findings without editing code. The
dispatcher updates spec and roadmap status only when the evidence supports it.

### Report Progress

```text
/roadmap-status status=all
```

This reports current focus, completed work, blockers, deferred work, and the next
actionable feature.

## Mini Example

Assume the project idea is a mobile-first application that asks an adult about a
child and the current setting, then generates suitable games such as riddles,
treasure hunts, and movement activities.

### Project Spec Excerpt

```markdown
## Application Purpose

* Help an adult find age-appropriate games for the current place, available
  time, group size, energy level, and interests.

## Target Users

* Parent, educator, or caregiver choosing an activity.

## Core Workflows

1. Enter child and activity context.
2. Generate a requested number of suitable games.
3. Open instructions, materials, duration, and variants.
```

### Roadmap Excerpt

| ID    | Feature                  | User Outcome                    | Status      | Dependencies |
|-------|--------------------------|---------------------------------|-------------|--------------|
| F-001 | Activity intake          | Describe the current context    | `Planned`   | None         |
| F-002 | Game generation          | Receive suitable game ideas     | `Candidate` | F-001        |
| F-003 | Game results and details | Follow clear game instructions  | `Candidate` | F-002        |
| F-004 | Saved games              | Revisit favorite activities     | `Deferred`  | F-003        |

### Feature Spec Excerpt

```markdown
## Feature

ID: `F-001`

Name: `Activity intake`

Status: `Planned`

## Acceptance Criteria

* [ ] The adult can provide age range, place, available time, group size,
  energy level, interests, and requested game count.
* [ ] Required fields show accessible validation messages.
* [ ] The form works at a 320 px viewport without horizontal scrolling.
* [ ] Component and route tests cover valid submission and validation errors.

## Implementation Plan

1. Create a route that owns form submission and navigation.
2. Create reusable fields for option selection and game count.
3. Add accessible validation and responsive styles.
4. Add component and route tests.
```

The user reviews this spec and invokes `/feature-implement featureId=F-001`.
The React specialist implements the form and tests. The validator then checks
the acceptance criteria and responsive behavior before the dispatcher marks the
feature `Validated`.

## Working Agreements

* Specs and roadmap changes are committed with the code they describe
* Every implementation includes relevant tests
* `Implemented` and `Validated` remain separate states
* Assessment does not silently repair findings
* Feature specs record deviations and validation evidence
* Natural conversation remains available when a slash prompt is unnecessary

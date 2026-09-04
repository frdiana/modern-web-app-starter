---
name: feature-spec
description: "Create or refine an implementation-ready feature spec and roadmap entry without writing production code - Brought to you by frdiana/modern-web-app-starter"
agent: "Modern Web App Architect"
argument-hint: "featureId={F-001|next} feature=..."
---

# Specify Feature

## Inputs

* `${input:featureId:next}`: Existing feature ID or `next` to allocate one
* `${input:feature}`: Required feature goal or roadmap feature name

## Requirements

Follow the feature specification steps from `Modern Web App Architect`.
Delegate planning to `Modern Product Spec Architect`, update the roadmap, and
create `.specs/features/<feature-id>-<feature-slug>/spec.md`. Stop for user
approval when the spec reaches `Planned`. Do not implement production code.

> Brought to you by frdiana/modern-web-app-starter
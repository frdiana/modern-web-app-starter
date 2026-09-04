---
name: roadmap-status
description: "Summarize and reconcile project feature progress from the roadmap and feature specs without implementing code - Brought to you by frdiana/modern-web-app-starter"
agent: "Modern Web App Architect"
argument-hint: "[status={all|candidate|planned|active|blocked}]"
---

# Report Roadmap Status

## Inputs

* `${input:status:all}`: Optional status group to emphasize

## Requirements

Read `.specs/project.md`, `.specs/roadmap.md`, and all referenced feature specs.
Reconcile roadmap entries only when feature specs provide direct evidence of a
stale status. Report progress, current focus, blockers, validated features, and
the next actionable feature. Do not modify production code or tests.

> Brought to you by frdiana/modern-web-app-starter
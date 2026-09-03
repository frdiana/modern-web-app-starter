---
name: add-endpoint
description: "Add and test a vertical slice API endpoint - Brought to you by frdiana/verticalslice.template"
agent: "Vertical Slice Architect"
argument-hint: "resource=... operation=... route=... behavior=..."
---

# Add Endpoint

## Inputs

* `${input:resource}`: Required API resource name
* `${input:operation}`: Required operation name such as `Get`, `Create`, or
  `Delete`
* `${input:route}`: Required HTTP route
* `${input:behavior}`: Required business behavior and expected outcomes

## Requirements

Implement the endpoint through the complete Vertical Slice Architect workflow.
Preserve project boundaries, create meaningful tests, build the solution, and
report any contract decision that could not be inferred from existing code.

> Brought to you by frdiana/verticalslice.template
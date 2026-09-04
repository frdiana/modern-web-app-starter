---
name: feature-assess
description: "Independently assess an implemented feature against its spec, acceptance criteria, and executable checks - Brought to you by frdiana/modern-web-app-starter"
agent: "Modern Web App Architect"
argument-hint: "featureId=F-001"
---

# Assess Feature

## Inputs

* `${input:featureId}`: Required implemented feature ID

## Required Protocol

Delegate assessment to `Modern Feature Validator`. Do not modify production
code or tests and do not fix findings during assessment. After reviewing the
validator evidence, update only the feature spec and roadmap with the justified
status, findings, commands run, and residual risks.

> Brought to you by frdiana/modern-web-app-starter
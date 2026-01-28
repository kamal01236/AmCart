# Audit Requirements

## Interim Demo Progress
- Architecture documentation (HLD/LLD) updated with Azure AKS, Application Gateway WAF, Azure AD/B2C identity, Azure Event Hubs, PostgreSQL, Redis, and Key Vault integrations.
- Decision records captured in docs/decisions/ (microservices, API gateway, identity provider, forthcoming AKS/Event Hubs/Angular ADRs).
- Requirements and assumptions clarified in docs/requirements/ for interim scope (SPA + four .NET services) to enable traceability.

## Required Artifacts
| Area | Artifact | Location |
| --- | --- | --- |
| Architecture | HLD, LLD, diagrams, ADRs | docs/design/, architecture/diagrams/, docs/decisions/ |
| Security | Identity config, gateway policies, Key Vault runbooks | security/auth/, security/api-gateway/, security/secrets/ |
| Infrastructure | Terraform plans, state handling docs | infrastructure/regions/, infrastructure/modules/ |
| CI/CD | GitHub Actions workflows, release checklist | .github/workflows/, pipelines/ |
| Testing | Unit/integration coverage reports | services/*-service/tests/, applications/web-app/tests/ |
| Compliance | Runbooks, audit evidence | docs/compliance/ |

## Controls to Demonstrate
- Identity & Access: Azure AD Conditional Access, role-based scopes, managed identities per workload.
- Change Management: GitHub workflow approvals, artifacts signed and promoted into AKS environments.
- Data Protection: Key Vault-managed secrets, TLS everywhere, encryption-at-rest enabled on Postgres/Redis/Event Hubs.
- Observability: Centralized logging, metrics, tracing, and alerting evidence stored in observability/.
- Deployment Traceability: Code change → build → deploy proof captured via GitHub Actions logs and release notes.
# ADR-003: Azure AD Identity Provider

- Status: Accepted (2026-01-28)

## Context
The platform must deliver secure login for customers and administrators, integrate tightly with Azure services (AKS, Key Vault, Application Gateway), and support GitHub Actions deployments without long-lived secrets. Centralized governance, Conditional Access, MFA, and device compliance all need to live within Microsoft Entra ID (Azure AD), and customer identities require Azure AD B2C policies. Selecting Azure AD across all personas removes external identity dependencies and aligns with enterprise mandates.

## Decision
Adopt Microsoft Entra ID (Azure AD) as the single identity provider for administrators, operators, and CI/CD federations, and Microsoft Entra ID B2C for customer identities. All OAuth 2.0 / OIDC flows use Azure AD-issued tokens (Authorization Code + PKCE for the SPA, client credentials/managed identities for services). Terraform provisions app registrations, roles/scopes, managed identities, and GitHub OIDC federations as part of the infrastructure codebase.

## Consequences
- Unified governance: Conditional Access, MFA, device policies, and audit logging remain inside Azure AD / Azure AD B2C.
- Simplified operations: No third-party IdP lifecycle; identity artifacts are versioned via Terraform modules.
- Security posture: CI/CD relies on Azure AD federated credentials; runtime workloads use managed identities to reach Key Vault and data services.
- Additional work: Teams must manage Azure AD app registrations, B2C user flows, role assignments, and periodic certificate/key rotations through automation and documented runbooks.

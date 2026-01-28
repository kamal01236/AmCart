# Azure AD / Azure AD B2C Configuration

## Overview
Microsoft Entra ID (Azure AD) provides identities for admins/operators and CI/CD. Azure AD B2C hosts customer identities. This document captures tenant setup, app registrations, scopes/roles, PKCE configuration for the SPA, and GitHub Actions OIDC federation. All values are managed via Terraform modules under infrastructure/modules/identity/ with environment-specific variables.

## Tenants & Directories
- **Azure AD (core tenant)**: hosts administrator accounts, service principals, managed identities, and CI/CD federations.
- **Azure AD B2C tenant**: stores customer accounts, sign-up/sign-in policies, password reset policies, and MFA configuration.

## App Registrations
| App | Tenant | Purpose | Notes |
| --- | --- | --- | --- |
| `amcart-spa` | B2C | Angular web app | Front-channel OAuth2 Authorization Code + PKCE; reply URLs set per environment (`https://<env>.amcart.com/auth/callback`). |
| `amcart-api` | Core | Protects microservices | Exposes scopes (`Catalog.Read`, `Order.Write`, etc.) and app roles (`Customer`, `Admin`). |
| `amcart-ci` | Core | GitHub Actions federation | Receives federated credential definitions for workflows; no client secret. |

Terraform provisions each registration, exports client IDs, and wires redirect URIs. Secrets are not issued; managed identities and OIDC are preferred.

## SPA Configuration (Angular)
- Use MSAL.js with Authorization Code + PKCE against Azure AD B2C user flows.
- Required configuration keys (provided via ConfigMap/Key Vault):
- `tenantId`, `clientId`, `authority` (B2C user flow endpoint), `redirectUri`, `postLogoutRedirectUri`, `scopes` (API scopes exposed by `amcart-api`).
- Enforce silent refresh via refresh tokens, with fallback to interactive login.

## API Gateway & Microservices
- API Gateway validates incoming JWTs using Azure AD signing keys; reject tokens missing required `scp` or `roles` claims.
- Downstream services use Microsoft.Identity.Web to validate tokens and enforce role checks.
- Managed identities assigned per namespace retrieve secrets from Azure Key Vault and authenticate to Azure services (PostgreSQL, Event Hubs, Storage).

## Customer vs Admin Identities
- **Customers**: created in Azure AD B2C. User flows/policies defined for sign-up/sign-in, profile edit, and password reset.
- **Administrators**: native Azure AD accounts grouped (e.g., `AmCart-Admins`); group membership drives role claims.

## CI/CD Federation (GitHub Actions)
- Use Azure AD federated credentials: map GitHub repo/environment to Azure AD application `amcart-ci`.
- Workflow obtains access token via `azure/login` OIDC; no long-lived secrets stored in GitHub.

## Conditional Access & MFA
- Apply Conditional Access policies enforcing MFA for admin accounts, blocking legacy protocols, and restricting login to compliant devices.
- Customers leverage Azure AD B2C built-in MFA or SMS/email OTP flows depending on regulatory needs.

## Key Vault Integration
- Store connection strings, certificates, and third-party secrets in per-service Key Vault instances.
- AKS workloads access secrets via CSI driver using managed identities; no secrets in environment variables or GitHub.

## Required Terraform Inputs
- Tenant IDs and domain names (core + B2C).
- List of redirect URIs per environment.
- GitHub organization/repo identifiers for federated credentials.
- Role/scope definitions per API.

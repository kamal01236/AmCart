# ADR-006: Angular SPA & .NET 8 Microservices

- Status: Accepted (2026-01-28)

## Context
The platform must deliver a responsive SPA, integrate with Azure AD B2C, and share contracts with multiple backend services. Teams have existing expertise with Angular and ASP.NET Core. Requirements include enterprise component libraries, strong typing, RxJS streams, and native support for OpenAPI-driven clients, while backend services need high performance, async messaging, and alignment with Azure tooling.

## Decision
Build the customer/administrator web experience using Angular (latest LTS) with NgRx state management, MSAL for Azure AD B2C, and Cypress/Jest testing. Implement backend services (auth, catalog, order, payment) with ASP.NET Core (.NET 8), CQRS + MediatR, EF Core, and Azure Event Hubs SDKs. Share schema and API contracts via OpenAPI definitions stored alongside each service.

## Consequences
- Pros: Mature tooling, TypeScript/ C# alignment, rich ecosystem for testing and linting, first-class Azure AD integrations, and container-friendly builds.
- Cons: Larger bundle size compared to lightweight frameworks; .NET services require Linux containers and multi-stage builds to optimize image sizes.
- Actions: applications/web-app/ hosts Angular workspace with modular architecture; services/*-service contain .NET solutions with Dockerfiles/Helm charts; CI/CD pipelines run lint/test builds for both stacks and deploy to AKS.

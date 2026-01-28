1. Assumptions (Explicit & Documented)

These assumptions were made to resolve ambiguities in the original requirements and to enable architectural progress. None of them contradict the stated requirements.

1.1 Business & Scope Assumptions

The application is a B2C e-commerce platform for clothing and accessories.

Customers and administrators are distinct roles, not distinct user types.

The application initially targets web users; mobile apps may be added later.

The platform will support global users.

Peak traffic is expected during sales and promotions.

1.2 UI & Application Assumptions

A single web application (SPA) will serve both customers and administrators.

Admin and customer functionality will be controlled using role-based access control (RBAC).

A separate admin portal is not required by current requirements, but the architecture allows it in the future.

Users will not access backend services directly.

1.3 Security Assumptions

OAuth 2.0 / OpenID Connect with Microsoft Entra ID (Azure AD) is the standard for authentication.

Azure AD B2C hosts customer identities; Azure AD (core tenant) hosts admins, service principals, and managed identities.

All external communication must use HTTPS.

Backend services trust the API Gateway, not the client.

Authorization decisions are enforced at the API Gateway and service level.

GitHub Actions uses OIDC federation with Azure AD—no long-lived secrets.

1.4 Data & Consistency Assumptions

Orders, payments, and inventory require strong consistency.

Reviews, wishlists, notifications, and analytics can tolerate eventual consistency.

Redis is used strictly as a cache, never as a system of record.

Elasticsearch is a search index only, not a source of truth.

Each microservice owns its data and database schema.

1.5 Architecture & Deployment Assumptions

The system is cloud-native and deployed on managed cloud services.

Microservices are containerized and deployed on Kubernetes.

CI/CD pipelines are mandatory for all services.

The system may be deployed in multiple regions for availability.

SQL databases use single-writer, multi-reader strategy in multi-region.

No synchronous cross-region service calls are allowed.

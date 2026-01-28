| Requirement            | Type        | Design Decision / Component           |
|------------------------|-------------|---------------------------------------|
| Secure authentication  | Functional  | Auth0, API Gateway                   |
| Role-based access      | Functional  | JWT claims, API Gateway              |
| Product search         | Functional  | Elasticsearch                        |
| Order placement        | Functional  | Order Service, PostgreSQL            |
| Payment processing     | Functional  | Payment Service, PostgreSQL          |
| Inventory accuracy     | Functional  | Inventory Service, PostgreSQL        |
| Reviews & wishlists    | Functional  | Cassandra                            |
| High availability      | NFR         | Multi-region deployment              |
| Scalability            | NFR         | Microservices, AKS                   |
| Low latency            | NFR         | Redis, CDN                           |
| Fault isolation        | NFR         | Kafka, microservices                 |
| Security compliance    | NFR         | TLS, OAuth2, zero trust              |
| Maintainability        | NFR         | Modular services, CI/CD              |

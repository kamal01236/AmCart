3. Non-Functional Requirements (How the System Must Behave)
3.1 Security

All communication must use SSL/TLS.

User sessions must expire after inactivity.

Sensitive data must be encrypted in transit.

No passwords must be stored in cookies.

Backend systems must be accessible only to authorized users.

3.2 Performance

Product browsing and search must be low-latency.

The system must support concurrent users.

Cache must be used to reduce database load.

3.3 Availability & Reliability

The system must be available 24×7.

The system must tolerate partial failures.

The system must support failover in case of regional outages.

Data must be backed up regularly.

3.4 Scalability

The system must scale horizontally.

The system must handle traffic spikes during promotions.

Read and write workloads must scale independently.

3.5 Maintainability

The system must be modular and loosely coupled.

Services must be independently deployable.

CI/CD pipelines must automate build and deployment.

Configuration must be externalized.

3.6 Observability

The system must provide centralized logging.

Metrics and health checks must be available.

Errors must be traceable across services.

3.7 Portability

The application must run on multiple operating systems.

The application must be accessible via standard web browsers.

Cloud vendor lock-in must be minimized.

4. One-Page Summary (Reviewer-Ready)

The system requirements define a secure, scalable, and highly available e-commerce platform. Functional requirements focus on user experience, commerce flows, and administration, while non-functional requirements emphasize security, performance, scalability, and reliability. Explicit assumptions have been documented to resolve ambiguities and ensure traceability between requirements and architectural decisions.
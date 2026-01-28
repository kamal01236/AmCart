# ADR-004: Azure Kubernetes Service

- Status: Accepted (2026-01-28)

## Context
The platform requires elastic compute for multiple .NET microservices, rolling updates, managed identity support, and integration with Azure networking, Key Vault, and monitoring. Previous container hosting options (VM Scale Sets, App Service for Containers) lacked unified ingress, pod identity, and workload isolation features needed for the interim/final demos.

## Decision
Use Azure Kubernetes Service (AKS) as the primary runtime for all microservices and supporting workloads. AKS will run inside a dedicated subnet with Azure CNI networking, expose ingress through Azure Application Gateway (AGIC), and leverage managed identities for pod-level access to Azure resources.

## Consequences
- Pros: Managed control plane, native integration with Azure Monitor/Key Vault, autoscaling, support for Windows/Linux node pools, and GitHub OIDC support via `az` CLI.
- Cons: Requires Kubernetes expertise, ongoing cluster patching/upgrade cadence, and additional governance (namespaces, network policies, quota management).
- Actions: Terraform modules provision AKS, node pools, diagnostics, and RBAC; deployment manifests in deployment/kubernetes/ must align with AGIC annotations and pod identity bindings.

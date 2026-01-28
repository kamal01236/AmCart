# CI/CD Workflow Guide

## Pipeline Inventory
- **Build - Web**: pipelines/ci/build-web.yml compiles/lints/tests the Angular SPA and publishes artifacts tagged per commit SHA.
- **Build - Services**: pipelines/ci/build-services.yml runs dotnet format/test/publish for every microservice container, pushing OCI images to Azure Container Registry (ACR) with semver + commit metadata.
- **Deploy - Dev**: pipelines/cd/deploy-dev.yml orchestrates Terraform + Helm for dev across eastus then westeurope.
- **Deploy - Prod**: pipelines/cd/deploy-prod.yml mirrors the dev workflow but gates on staging verification, change management, and SRE approval checks.

## Multi-Region Orchestration
- Workflows run per environment and fan in to a `deploy-{env}` reusable workflow containing two explicit regions: eastus then westeurope. Job-level concurrency keys (`infra-${{ env }}-eastus`, etc.) prevent overlapping applies.
- Terraform plan/apply uses infrastructure/environments/<env>-<region>.tfvars and distinct remote state containers per region (`tfstate/{env}/{region}/infra.tfstate`), ensuring blast radius stays local.
- After the eastus job completes, a health verification job (synthetic checks + smoke API tests) must succeed before westeurope starts. Failure keeps westeurope pinned to the previous release while alerts page SREs.

## Deployment Flow
1. **Plan Phase**: `terraform fmt && validate` → remote backend init → plan uploaded as artifact for human approval (prod) or auto-approve (dev/staging).
2. **Apply Phase**: eastus applies first, updating networking, AKS, and data services; artifacts capture Key Vault version updates and App Gateway listener diffs. On success, westeurope repeats with identical modules but its own tfvars.
3. **Helm Release**: Once infrastructure is consistent, Helm deploys run via pipelines/templates/helm-deploy.yml using the image digests produced by CI. Releases target the AKS cluster in the same region that just completed infra apply.
4. **Traffic Management**: Azure Traffic Manager weights default to $70\%$ eastus / $30\%$ westeurope. During deploys the workflow can temporarily drain westeurope (set to $10\%$) while eastus rolls forward, then restore the steady-state mix after regional health checks.

## Promotion & Rollback
- Promotions move sequentially: dev → staging → prod. Each promotion reuses the same artifacts to guarantee immutability.
- Rollbacks are region-aware: workflows track the previous Helm release per region and provide a `rollback-region` input that redeploys the last known-good chart and restores Traffic Manager weights.
- Observability hooks (Log Analytics query + Application Insights availability test) must pass for 15 minutes post-deploy before the workflow marks the region healthy and hands off to the next stage.

## Security & Compliance
- GitHub Actions authenticates to Azure using OIDC federated credentials defined in infrastructure/modules/identity/github-oidc. No secrets are stored in the repo.
- Every deploy job emits evidence (plan, apply, Helm diff, test output) stored in the workflow summary to satisfy audit requirements documented in docs/compliance/audit-requirements.md.

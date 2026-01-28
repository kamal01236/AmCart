# Global Azure AD Stack

This Terraform stack provisions the shared Microsoft Entra ID (Azure AD) and Azure AD B2C assets used by AmCart:

- SPA app registration (B2C) for the Angular web app
- API app registration (core tenant) exposing OAuth2 scopes and app roles
- GitHub Actions OIDC application with federated credentials per workflow
- Optional Key Vault secrets so downstream services can consume client IDs, scope URIs, and B2C authorities without hardcoding values.

## Usage

```bash
cd infrastructure/global/aad
terraform init
terraform plan -var-file=../../environments/dev-global.tfvars
```

Required variables (see `infrastructure/environments/*-global.tfvars` for samples):

- `core_tenant_id` / `core_tenant_domain`
- `b2c_tenant_id` / `b2c_tenant_domain`
- `spa_redirect_uris` + `spa_logout_url`
- `api_identifier_uri`, `api_scopes`, and `api_roles`
- `github_federations` describing each GitHub Actions workflow that needs tenant access
- `config_key_vault_id` for the Key Vault that stores generated client IDs/scopes (set to `null` to skip secret writes).

Outputs include the MSAL authority, client IDs, scope URIs, and populated Key Vault secret names so that Kubernetes manifests, Helm values, and application settings can reference the correct entries.

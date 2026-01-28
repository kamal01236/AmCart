output "spa_client_id" {
  description = "Client ID for the SPA"
  value       = module.spa_app.client_id
}

output "api_client_id" {
  description = "Client ID for the protected API"
  value       = module.api_app.client_id
}

output "api_identifier_uri" {
  description = "Application ID URI exposed by the API"
  value       = var.api_identifier_uri
}

output "api_scope_uris" {
  description = "Fully qualified scope URIs"
  value       = local.scope_uri_map
}

output "ci_app_client_id" {
  description = "Client ID for the GitHub OIDC application"
  value       = module.ci_app.client_id
}

output "key_vault_secret_names" {
  description = "Names of Key Vault secrets populated by this stack"
  value       = var.config_key_vault_id == null ? [] : keys(local.identity_secret_values)
}

output "spa_authority" {
  description = "Authority URL for the SPA MSAL configuration"
  value       = local.spa_authority
}

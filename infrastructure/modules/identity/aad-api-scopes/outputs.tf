output "app_role_ids" {
  description = "Map of app role values to object IDs"
  value       = { for k, v in azuread_application_app_role.roles : k => v.id }
}

output "scope_ids" {
  description = "Map of OAuth2 scope values to object IDs"
  value       = { for k, v in azuread_application_oauth2_permission_scope.scopes : k => v.id }
}

output "managed_identity_client_ids" {
  description = "Client IDs for each managed identity"
  value       = { for key, mod in module.managed_identities : key => mod.client_id }
}

output "managed_identity_principal_ids" {
  description = "Principal IDs for each managed identity"
  value       = { for key, mod in module.managed_identities : key => mod.principal_id }
}

output "managed_identity_resource_ids" {
  description = "Resource IDs for each managed identity"
  value       = { for key, mod in module.managed_identities : key => mod.id }
}

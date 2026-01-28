terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 2.48.0"
    }
  }
}

resource "azuread_application_app_role" "roles" {
  for_each        = { for role in var.app_roles : role.value => role }
  application_id  = var.application_id
  allowed_member_types = each.value.allowed_member_types
  description     = each.value.description
  display_name    = each.value.display_name
  enabled         = true
  value           = each.value.value
}

resource "azuread_application_oauth2_permission_scope" "scopes" {
  for_each                   = { for scope in var.oauth2_scopes : scope.value => scope }
  application_id             = var.application_id
  admin_consent_description  = each.value.admin_consent_description
  admin_consent_display_name = each.value.admin_consent_display_name
  enabled                    = true
  type                       = each.value.type
  user_consent_description   = each.value.user_consent_description
  user_consent_display_name  = each.value.user_consent_display_name
  value                      = each.value.value
}

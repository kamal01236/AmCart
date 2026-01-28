terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 2.48.0"
    }
  }
}

resource "azuread_app_role_assignment" "assignments" {
  for_each            = { for assignment in var.assignments : "${assignment.principal_object_id}-${assignment.app_role_id}" => assignment }
  app_role_id         = each.value.app_role_id
  principal_object_id = each.value.principal_object_id
  resource_object_id  = var.service_principal_object_id
}

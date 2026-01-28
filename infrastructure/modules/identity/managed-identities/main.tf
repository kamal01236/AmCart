terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.90.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_user_assigned_identity" "this" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

output "client_id" {
  description = "Managed identity client ID"
  value       = azurerm_user_assigned_identity.this.client_id
}

output "principal_id" {
  description = "Managed identity principal ID"
  value       = azurerm_user_assigned_identity.this.principal_id
}

output "id" {
  description = "Managed identity resource ID"
  value       = azurerm_user_assigned_identity.this.id
}

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

locals {
  base_tags = merge({
    "amcart:environment" = var.environment,
    "amcart:region"      = var.region
  }, var.tags)
}

module "managed_identities" {
  for_each = var.service_managed_identities
  source   = "../../../modules/identity/managed-identities"

  name                = each.value.name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = merge(local.base_tags, lookup(each.value, "tags", {}))
}
*** End of File
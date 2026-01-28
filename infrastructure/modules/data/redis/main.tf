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

resource "azurerm_redis_cache" "this" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  capacity            = var.capacity
  family              = var.family
  sku_name            = var.sku
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  shard_count         = var.shard_count
  tags                = var.tags
}

output "host_name" {
  description = "Redis hostname"
  value       = azurerm_redis_cache.this.hostname
}

output "primary_key" {
  description = "Primary access key"
  value       = azurerm_redis_cache.this.primary_access_key
  sensitive   = true
}

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

resource "azurerm_eventhub_namespace" "this" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  capacity            = var.capacity
  auto_inflate_enabled = var.auto_inflate
  maximum_throughput_units = var.max_throughput_units
  tags                = var.tags
}

resource "azurerm_eventhub" "eventhubs" {
  for_each            = { for hub in var.eventhubs : hub.name => hub }
  name                = each.value.name
  namespace_name      = azurerm_eventhub_namespace.this.name
  resource_group_name = var.resource_group_name
  partition_count     = each.value.partition_count
  message_retention   = each.value.retention_days
}

resource "azurerm_eventhub_consumer_group" "consumer_groups" {
  for_each            = { for cg in var.consumer_groups : "${cg.eventhub_name}-${cg.name}" => cg }
  name                = each.value.name
  namespace_name      = azurerm_eventhub_namespace.this.name
  eventhub_name       = each.value.eventhub_name
  resource_group_name = var.resource_group_name
}

output "namespace_connection_string" {
  description = "Primary connection string"
  value       = azurerm_eventhub_namespace.this.default_primary_connection_string
  sensitive   = true
}

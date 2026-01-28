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

resource "azurerm_postgresql_flexible_server" "this" {
  name                   = var.name
  resource_group_name    = var.resource_group_name
  location               = var.location
  version                = var.version
  delegated_subnet_id    = var.subnet_id
  administrator_login    = var.admin_login
  administrator_password = var.admin_password
  zone                   = var.zone
  storage_mb             = var.storage_mb
  sku_name               = var.sku_name
  backup_retention_days  = var.backup_retention_days
  high_availability {
    mode = var.ha_mode
  }
  tags = var.tags
}

resource "azurerm_postgresql_flexible_server_database" "databases" {
  for_each            = toset(var.databases)
  name                = each.value
  server_name         = azurerm_postgresql_flexible_server.this.name
  resource_group_name = var.resource_group_name
  charset             = "UTF8"
  collation           = "en_US.utf8"
}

output "server_id" {
  description = "PostgreSQL server ID"
  value       = azurerm_postgresql_flexible_server.this.id
}

output "connection_string" {
  description = "ADO.NET style connection string"
  value       = "Host=${azurerm_postgresql_flexible_server.this.fqdn};Database=${var.databases[0]};Username=${var.admin_login};Password=${var.admin_password};"
  sensitive   = true
}

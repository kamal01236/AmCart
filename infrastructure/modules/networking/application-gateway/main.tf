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

resource "azurerm_public_ip" "appgw" {
  name                = "${var.name}-pip"
  resource_group_name = var.resource_group_name
  location            = var.location
  allocation_method   = "Static"
  sku                 = "Standard"
  sku_tier            = "Regional"
  tags                = var.tags
}

resource "azurerm_application_gateway" "this" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags

  sku {
    name     = "WAF_v2"
    tier     = "WAF_v2"
    capacity = var.capacity
  }

  gateway_ip_configuration {
    name      = "gateway-ip"
    subnet_id = var.subnet_id
  }

  frontend_port {
    name = "https"
    port = 443
  }

  frontend_ip_configuration {
    name                 = "public-frontend"
    public_ip_address_id = azurerm_public_ip.appgw.id
  }

  backend_address_pool {
    name = "aks-ingress"
  }

  http_listener {
    name                           = "https-listener"
    frontend_ip_configuration_name = "public-frontend"
    frontend_port_name             = "https"
    protocol                       = "Https"
    ssl_certificate_name           = var.ssl_certificate_name
  }

  request_routing_rule {
    name                       = "default-routing"
    rule_type                  = "Basic"
    http_listener_name         = "https-listener"
    backend_address_pool_name  = "aks-ingress"
    backend_http_settings_name = "default-settings"
  }

  backend_http_settings {
    name                  = "default-settings"
    cookie_based_affinity = "Disabled"
    port                  = 443
    protocol              = "Https"
    request_timeout       = 30
    host_name             = var.backend_hostname
    pick_host_name_from_backend_address = false
  }

  ssl_certificate {
    name     = var.ssl_certificate_name
    key_vault_secret_id = var.ssl_certificate_secret_id
  }

  waf_configuration {
    enabled          = true
    firewall_mode    = var.waf_mode
    rule_set_version = "3.2"
    rule_set_type    = "OWASP"
  }
}

output "application_gateway_id" {
  value       = azurerm_application_gateway.this.id
  description = "Application Gateway resource ID"
}

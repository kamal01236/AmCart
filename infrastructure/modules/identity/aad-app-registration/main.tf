terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 2.48.0"
    }
  }
}

resource "azuread_application" "this" {
  display_name     = var.display_name
  identifier_uris  = var.identifier_uris
  sign_in_audience = var.sign_in_audience

  optional_claims {
    access_token {
      name = "roles"
    }
    id_token {
      name = "roles"
    }
  }

  dynamic "web" {
    for_each = var.application_type == "web" ? [1] : []
    content {
      redirect_uris = var.redirect_uris
      logout_url    = var.logout_url
      implicit_grant {
        access_token_issuance_enabled = false
        id_token_issuance_enabled     = false
      }
    }
  }

  dynamic "spa" {
    for_each = var.application_type == "spa" ? [1] : []
    content {
      redirect_uris = var.redirect_uris
    }
  }
}

resource "azuread_service_principal" "this" {
  application_id = azuread_application.this.application_id
}

output "application_id" {
  description = "Azure AD application ID"
  value       = azuread_application.this.application_id
}

output "client_id" {
  description = "Azure AD client ID"
  value       = azuread_application.this.client_id
}

output "service_principal_id" {
  description = "Service principal object ID"
  value       = azuread_service_principal.this.id
}

output "object_id" {
  description = "Azure AD application object ID"
  value       = azuread_application.this.object_id
}

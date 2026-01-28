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

  web {
    redirect_uris = var.redirect_uris
    logout_url    = var.logout_url
    implicit_grant {
      access_token_issuance_enabled = false
      id_token_issuance_enabled     = false
    }
  }

  api {
    requested_access_token_version = 2
    oauth2_permission_scope {
      admin_consent_description  = "Access the AmCart APIs"
      admin_consent_display_name = "Access"
      enabled                    = true
      id                         = uuid()
      type                       = "User"
      user_consent_description   = "Access your AmCart data"
      user_consent_display_name  = "Access"
      value                      = "api.access"
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

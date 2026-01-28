terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 2.48.0"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.90.0"
    }
  }
}

provider "azuread" {
  tenant_id = var.core_tenant_id
}

provider "azuread" {
  alias     = "b2c"
  tenant_id = var.b2c_tenant_id
}

provider "azurerm" {
  features {}
}

locals {
  prefix           = var.application_prefix
  spa_display_name = "${local.prefix}-spa-${var.environment}"
  api_display_name = "${local.prefix}-api-${var.environment}"
  ci_display_name  = "${local.prefix}-ci-${var.environment}"
  spa_authority    = "https://${var.b2c_tenant_domain}/${var.b2c_user_flow}"
  scope_uri_map    = { for scope in var.api_scopes : scope.value => "${var.api_identifier_uri}/${scope.value}" }
  sanitized_scope  = { for scope, _ in local.scope_uri_map : scope => replace(lower(scope), ".", "-") }
  federated_creds  = [for cred in var.github_federations : {
    name         = cred.name
    display_name = cred.display_name
    issuer       = var.github_oidc_issuer
    subject      = cred.subject
    audiences    = cred.audiences
    description  = cred.description
  }]
}

module "spa_app" {
  source           = "../../modules/identity/aad-app-registration"
  providers        = { azuread = azuread.b2c }
  display_name     = local.spa_display_name
  identifier_uris  = []
  application_type = "spa"
  redirect_uris    = var.spa_redirect_uris
  logout_url       = var.spa_logout_url
  sign_in_audience = "AzureADandPersonalMicrosoftAccount"
}

module "api_app" {
  source           = "../../modules/identity/aad-app-registration"
  display_name     = local.api_display_name
  identifier_uris  = [var.api_identifier_uri]
  application_type = "web"
  redirect_uris    = []
  logout_url       = null
  sign_in_audience = "AzureADMyOrg"
}

module "ci_app" {
  source           = "../../modules/identity/aad-app-registration"
  display_name     = local.ci_display_name
  identifier_uris  = []
  application_type = "web"
  redirect_uris    = []
  logout_url       = null
  sign_in_audience = "AzureADMyOrg"
}

module "api_roles_scopes" {
  source          = "../../modules/identity/aad-api-scopes"
  application_id  = module.api_app.application_id
  app_roles       = var.api_roles
  oauth2_scopes   = var.api_scopes
}

module "github_oidc" {
  source                 = "../../modules/identity/federated-credentials"
  application_object_id  = module.ci_app.object_id
  credentials            = local.federated_creds
}

locals {
  identity_secret_values = merge(
    {
      "aad-spa-client-id-${var.environment}"       = module.spa_app.client_id,
      "aad-spa-application-id-${var.environment}"   = module.spa_app.application_id,
      "aad-api-client-id-${var.environment}"       = module.api_app.client_id,
      "aad-api-application-id-${var.environment}"  = module.api_app.application_id,
      "aad-api-app-id-uri-${var.environment}"      = var.api_identifier_uri,
      "aad-ci-client-id-${var.environment}"        = module.ci_app.client_id,
      "aad-b2c-authority-${var.environment}"       = local.spa_authority
    },
    { for scope, uri in local.scope_uri_map : "aad-scope-${local.sanitized_scope[scope]}-${var.environment}" => uri }
  )
}

resource "azurerm_key_vault_secret" "identity" {
  for_each     = var.config_key_vault_id == null ? {} : local.identity_secret_values
  name         = each.key
  value        = each.value
  key_vault_id = var.config_key_vault_id
  content_type = "text/plain"
}
*** End of File
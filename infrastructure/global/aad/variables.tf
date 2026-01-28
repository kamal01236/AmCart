variable "environment" {
  description = "Deployment environment (dev/staging/prod)"
  type        = string
}

variable "application_prefix" {
  description = "Prefix for Azure AD app registrations"
  type        = string
  default     = "amcart"
}

variable "core_tenant_id" {
  description = "Tenant ID for the core Azure AD directory"
  type        = string
}

variable "core_tenant_domain" {
  description = "Primary domain for the core Azure AD tenant"
  type        = string
}

variable "b2c_tenant_id" {
  description = "Tenant ID for the Azure AD B2C directory"
  type        = string
}

variable "b2c_tenant_domain" {
  description = "Primary domain for the Azure AD B2C tenant"
  type        = string
}

variable "b2c_user_flow" {
  description = "Name of the Azure AD B2C user flow used by the SPA"
  type        = string
  default     = "B2C_1_signupsignin"
}

variable "spa_redirect_uris" {
  description = "Reply URLs for the SPA"
  type        = list(string)
}

variable "spa_logout_url" {
  description = "Logout URL for the SPA (optional)"
  type        = string
  default     = null
}

variable "api_identifier_uri" {
  description = "Application ID URI for the protected APIs"
  type        = string
}

variable "api_scopes" {
  description = "OAuth2 scopes exposed by the API"
  type = list(object({
    value                      = string
    admin_consent_display_name = string
    admin_consent_description  = string
    user_consent_display_name  = string
    user_consent_description   = string
    type                       = string
  }))
}

variable "api_roles" {
  description = "App roles enforced by the API"
  type = list(object({
    value                = string
    display_name         = string
    description          = string
    allowed_member_types = list(string)
  }))
  default = []
}

variable "github_federations" {
  description = "GitHub Actions federated credential definitions"
  type = list(object({
    name         = string
    display_name = string
    subject      = string
    audiences    = list(string)
    description  = string
  }))
  default = []
}

variable "github_oidc_issuer" {
  description = "OIDC issuer for GitHub Actions"
  type        = string
  default     = "https://token.actions.githubusercontent.com"
}

variable "config_key_vault_id" {
  description = "Resource ID of the Key Vault that should store AAD config (optional)"
  type        = string
  default     = null
}

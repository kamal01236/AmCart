variable "application_id" {
  description = "Application (client) ID of the Azure AD app"
  type        = string
}

variable "app_roles" {
  description = "List of app roles to create"
  type = list(object({
    value                 = string
    display_name          = string
    description           = string
    allowed_member_types  = list(string)
  }))
  default = []
}

variable "oauth2_scopes" {
  description = "List of OAuth2 scopes exposed by the API"
  type = list(object({
    value                      = string
    admin_consent_display_name = string
    admin_consent_description  = string
    user_consent_display_name  = string
    user_consent_description   = string
    type                       = string
  }))
  default = []
}

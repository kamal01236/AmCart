variable "display_name" {
  description = "Display name for the Azure AD app registration"
  type        = string
}

variable "identifier_uris" {
  description = "Identifier URIs for the application"
  type        = list(string)
  default     = []
}

variable "redirect_uris" {
  description = "Reply URLs for web apps"
  type        = list(string)
  default     = []
}

variable "logout_url" {
  description = "Logout URL for the application"
  type        = string
  default     = null
}

variable "sign_in_audience" {
  description = "Signing audience (e.g., AzureADandPersonalMicrosoftAccount, AzureADMyOrg)"
  type        = string
  default     = "AzureADMultipleOrgs"
}

variable "application_type" {
  description = "Type of application to configure (web or spa)"
  type        = string
  default     = "web"
  validation {
    condition     = contains(["web", "spa"], var.application_type)
    error_message = "application_type must be either 'web' or 'spa'."
  }
}

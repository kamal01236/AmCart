variable "application_object_id" {
  description = "Object ID of the Azure AD application"
  type        = string
}

variable "credentials" {
  description = "Federated identity definitions"
  type = list(object({
    name        = string
    display_name = string
    issuer      = string
    subject     = string
    audiences   = list(string)
    description = string
  }))
}

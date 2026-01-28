variable "region" {
  description = "Azure region name"
  type        = string
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}

variable "location" {
  description = "Azure location for the managed identities"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group that will contain the managed identities"
  type        = string
}

variable "tags" {
  description = "Base tags applied to every managed identity"
  type        = map(string)
  default     = {}
}

variable "service_managed_identities" {
  description = "Definitions for each managed identity to create"
  type = map(object({
    name = string
    tags = optional(map(string), {})
  }))
}

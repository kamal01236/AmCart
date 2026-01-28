variable "name" {
  description = "Virtual network name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group"
  type        = string
}

variable "address_space" {
  description = "List of CIDR blocks"
  type        = list(string)
}

variable "subnets" {
  description = "Map of subnet definitions"
  type = map(object({
    address_prefixes                 = list(string)
    service_endpoints                = optional(list(string), [])
    private_endpoint_policies_enabled = optional(bool, false)
  }))
}

variable "tags" {
  description = "Tags applied to resources"
  type        = map(string)
  default     = {}
}

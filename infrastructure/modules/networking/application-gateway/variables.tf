variable "name" {
  type        = string
  description = "Application Gateway name"
}

variable "location" {
  type        = string
  description = "Azure region"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name"
}

variable "subnet_id" {
  type        = string
  description = "Subnet ID for the gateway"
}

variable "capacity" {
  type        = number
  description = "Instance count"
  default     = 2
}

variable "backend_hostname" {
  type        = string
  description = "Hostname used when routing to AKS ingress"
}

variable "ssl_certificate_name" {
  type        = string
  description = "Certificate alias"
  default     = "amcart-cert"
}

variable "ssl_certificate_secret_id" {
  type        = string
  description = "Key Vault secret ID containing the PFX"
}

variable "waf_mode" {
  type        = string
  description = "Detection or Prevention"
  default     = "Prevention"
}

variable "tags" {
  type        = map(string)
  description = "Resource tags"
  default     = {}
}

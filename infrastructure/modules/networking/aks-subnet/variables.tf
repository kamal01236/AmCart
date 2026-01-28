variable "subnet_id" {
  description = "AKS subnet ID"
  type        = string
}

variable "nsg_name" {
  description = "Network security group name"
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

variable "tags" {
  description = "Tags applied to the NSG"
  type        = map(string)
  default     = {}
}

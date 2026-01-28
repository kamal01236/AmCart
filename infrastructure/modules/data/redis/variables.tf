variable "name" {
  type        = string
  description = "Redis cache name"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group"
}

variable "location" {
  type        = string
  description = "Azure region"
}

variable "capacity" {
  type        = number
  description = "Redis capacity (0-6)"
  default     = 1
}

variable "family" {
  type        = string
  description = "C (basic/standard) or P (premium)"
  default     = "P"
}

variable "sku" {
  type        = string
  description = "SKU name"
  default     = "Premium"
}

variable "shard_count" {
  type        = number
  description = "Shard count for premium caches"
  default     = 0
}

variable "tags" {
  type        = map(string)
  description = "Resource tags"
  default     = {}
}

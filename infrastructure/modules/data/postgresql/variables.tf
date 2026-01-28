variable "name" {
  type        = string
  description = "PostgreSQL server name"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group"
}

variable "location" {
  type        = string
  description = "Azure region"
}

variable "subnet_id" {
  type        = string
  description = "Delegated subnet for the server"
}

variable "admin_login" {
  type        = string
  description = "Administrator username"
}

variable "admin_password" {
  type        = string
  description = "Administrator password"
  sensitive   = true
}

variable "databases" {
  type        = list(string)
  description = "Databases to create"
  default     = ["orders"]
}

variable "sku_name" {
  type        = string
  description = "SKU (e.g., GP_Standard_D2s_v3)"
  default     = "GP_Standard_D2s_v3"
}

variable "version" {
  type        = string
  description = "PostgreSQL version"
  default     = "15"
}

variable "storage_mb" {
  type        = number
  description = "Storage in MB"
  default     = 131072
}

variable "backup_retention_days" {
  type        = number
  description = "Backup retention"
  default     = 7
}

variable "ha_mode" {
  type        = string
  description = "High availability mode"
  default     = "ZoneRedundant"
}

variable "zone" {
  type        = string
  description = "Availability zone"
  default     = "1"
}

variable "tags" {
  type        = map(string)
  description = "Resource tags"
  default     = {}
}

variable "name" {
  type        = string
  description = "Event Hubs namespace name"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group"
}

variable "location" {
  type        = string
  description = "Azure region"
}

variable "sku" {
  type        = string
  description = "Standard or Dedicated"
  default     = "Standard"
}

variable "capacity" {
  type        = number
  description = "Throughput units"
  default     = 2
}

variable "auto_inflate" {
  type        = bool
  description = "Enable auto-inflate"
  default     = true
}

variable "max_throughput_units" {
  type        = number
  description = "Max TU when auto-inflate is enabled"
  default     = 20
}

variable "eventhubs" {
  description = "Event Hub definitions"
  type = list(object({
    name            = string
    partition_count = number
    retention_days  = number
  }))
}

variable "consumer_groups" {
  description = "Consumer group definitions"
  type = list(object({
    eventhub_name = string
    name          = string
  }))
  default = []
}

variable "tags" {
  type        = map(string)
  description = "Resource tags"
  default     = {}
}

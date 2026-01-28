variable "service_principal_object_id" {
  description = "Object ID of the service principal exposing the roles"
  type        = string
}

variable "assignments" {
  description = "List of role assignments"
  type = list(object({
    principal_object_id = string
    app_role_id         = string
  }))
}

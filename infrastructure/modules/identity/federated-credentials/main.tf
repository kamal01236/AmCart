terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = ">= 2.48.0"
    }
  }
}

resource "azuread_application_federated_identity_credential" "github" {
  for_each            = { for cred in var.credentials : cred.name => cred }
  application_object_id = var.application_object_id
  display_name          = each.value.display_name
  audiences             = each.value.audiences
  issuer                = each.value.issuer
  subject               = each.value.subject
  description           = each.value.description
}

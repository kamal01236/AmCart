# Azure AD App Role Assignments Module

Assigns Azure AD app roles to users, groups, or service principals.

```hcl
module "amcart_role_assignments" {
  source                       = "../../modules/identity/aad-roles"
  service_principal_object_id  = module.amcart_api_app.service_principal_id
  assignments = [
    {
      principal_object_id = azuread_group.admins.id
      app_role_id         = module.amcart_api_scopes.app_role_ids["Admin"]
    }
  ]
}
```

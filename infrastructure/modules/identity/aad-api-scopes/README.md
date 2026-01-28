# Azure AD API Scopes Module

Configures app roles and OAuth2 permission scopes for an existing Azure AD application.

```hcl
module "amcart_api_scopes" {
  source          = "../../modules/identity/aad-api-scopes"
  application_id  = module.amcart_api_app.application_id
  app_roles = [
    {
      value                = "Admin"
      display_name         = "Administrator"
      description          = "Full platform administration"
      allowed_member_types = ["User"]
    }
  ]
  oauth2_scopes = [
    {
      value                      = "Catalog.Read"
      admin_consent_display_name = "Read catalog"
      admin_consent_description  = "Allows read-only access to catalog APIs"
      user_consent_display_name  = "Read catalog"
      user_consent_description   = "Allows the app to read catalog data"
      type                       = "User"
    }
  ]
}
```

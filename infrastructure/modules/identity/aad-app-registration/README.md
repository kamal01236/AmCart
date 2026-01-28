# Azure AD App Registration Module

Creates an Azure AD application and service principal with optional redirect URIs and logout URL. Outputs the application ID, client ID, and service principal object ID so downstream modules (federated credentials, role assignments) can reference them.

```hcl
module "amcart_api_app" {
  source           = "../../modules/identity/aad-app-registration"
  display_name     = "amcart-api"
  identifier_uris  = ["api://amcart-api"]
  redirect_uris    = ["https://dev.amcart.com/auth/callback"]
  logout_url       = "https://dev.amcart.com/logout"
  sign_in_audience = "AzureADMyOrg"
}
```

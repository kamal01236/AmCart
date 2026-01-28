environment        = "prod"
application_prefix = "amcart"
core_tenant_id     = "11111111-1111-1111-1111-111111111111"
core_tenant_domain = "amcart.onmicrosoft.com"
b2c_tenant_id      = "22222222-2222-2222-2222-222222222222"
b2c_tenant_domain  = "amcartb2c.onmicrosoft.com"
b2c_user_flow      = "B2C_1_signupsignin"
spa_redirect_uris = [
  "https://www.amcart.com/auth/callback"
]
spa_logout_url   = "https://www.amcart.com/logout"
api_identifier_uri = "api://amcart-api"
api_scopes = [
  {
    value                      = "Catalog.Read"
    admin_consent_display_name = "Catalog Read"
    admin_consent_description  = "Read catalog records"
    user_consent_display_name  = "Catalog Read"
    user_consent_description   = "Allow the SPA to read catalog data."
    type                       = "User"
  },
  {
    value                      = "Catalog.Write"
    admin_consent_display_name = "Catalog Write"
    admin_consent_description  = "Modify catalog records"
    user_consent_display_name  = "Catalog Write"
    user_consent_description   = "Allow the SPA to create or update catalog data."
    type                       = "User"
  },
  {
    value                      = "Order.Read"
    admin_consent_display_name = "Order Read"
    admin_consent_description  = "Read customer orders"
    user_consent_display_name  = "Order Read"
    user_consent_description   = "Allow the SPA to view order history."
    type                       = "User"
  },
  {
    value                      = "Order.Write"
    admin_consent_display_name = "Order Write"
    admin_consent_description  = "Place and update orders"
    user_consent_display_name  = "Order Write"
    user_consent_description   = "Allow the SPA to place orders."
    type                       = "User"
  }
]
api_roles = [
  {
    value                = "Catalog.Admin"
    display_name         = "Catalog Admin"
    description          = "Manage catalog items, categories, and pricing"
    allowed_member_types = ["User", "Application"]
  },
  {
    value                = "Order.Admin"
    display_name         = "Order Admin"
    description          = "Override and fulfill orders"
    allowed_member_types = ["User", "Application"]
  },
  {
    value                = "Support.Agent"
    display_name         = "Support Agent"
    description          = "Assist customers with catalog and order issues"
    allowed_member_types = ["User"]
  }
]
github_federations = [
  {
    name         = "deploy-prod"
    display_name = "Deploy Prod"
    subject      = "repo:acme/amcart:environment:prod"
    audiences    = ["api://AzureADTokenExchange"]
    description  = "GitHub Actions workflow deploying production regions"
  }
]
config_key_vault_id = "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/rg-amcart-shared-prod/providers/Microsoft.KeyVault/vaults/kv-amcart-shared-prod"

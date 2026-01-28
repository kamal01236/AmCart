# Azure AD Federated Credentials Module

Creates federated identity credentials (e.g., GitHub Actions OIDC) for an Azure AD application.

```hcl
module "amcart_ci_oidc" {
  source                 = "../../modules/identity/federated-credentials"
  application_object_id  = module.amcart_ci_app.service_principal_id
  credentials = [
    {
      name         = "github-actions-dev"
      display_name = "GitHub Actions Dev"
      issuer       = "https://token.actions.githubusercontent.com"
      subject      = "repo:amcart/platform:environment:Dev"
      audiences    = ["api://AzureADTokenExchange"]
      description  = "Deployments from GitHub Actions to dev subscription"
    }
  ]
}
```

# Managed Identity Module

Creates an Azure user-assigned managed identity that AKS workloads or Azure services can bind to.

```hcl
module "order_identity" {
  source              = "../../modules/identity/managed-identities"
  name                = "id-order-service"
  resource_group_name = azurerm_resource_group.platform.name
  location            = azurerm_resource_group.platform.location
  tags                = var.tags
}
```

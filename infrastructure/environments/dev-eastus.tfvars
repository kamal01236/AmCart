location        = "eastus"
resource_group  = "rg-amcart-dev-eastus"
environment     = "dev"
region          = "eastus"
aks_node_count  = 2
postgres_tier   = "GP_Standard_D2s_v3"
redis_capacity  = 1
kafka_capacity  = 2
key_vault_name  = "kv-amcart-dev-eastus"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-dev-eastus"
	}
	catalog = {
		name = "mi-amcart-catalog-dev-eastus"
	}
	order = {
		name = "mi-amcart-order-dev-eastus"
	}
	search = {
		name = "mi-amcart-search-dev-eastus"
	}
}

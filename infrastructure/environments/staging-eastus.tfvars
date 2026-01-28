location        = "eastus"
resource_group  = "rg-amcart-staging-eastus"
environment     = "staging"
region          = "eastus"
aks_node_count  = 3
postgres_tier   = "GP_Standard_D4s_v3"
redis_capacity  = 2
kafka_capacity  = 3
key_vault_name  = "kv-amcart-staging-eastus"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-staging-eastus"
	}
	catalog = {
		name = "mi-amcart-catalog-staging-eastus"
	}
	order = {
		name = "mi-amcart-order-staging-eastus"
	}
	search = {
		name = "mi-amcart-search-staging-eastus"
	}
}

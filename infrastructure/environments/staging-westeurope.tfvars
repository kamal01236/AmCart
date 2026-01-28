location        = "westeurope"
resource_group  = "rg-amcart-staging-westeurope"
environment     = "staging"
region          = "westeurope"
aks_node_count  = 3
postgres_tier   = "GP_Standard_D4s_v3"
redis_capacity  = 2
kafka_capacity  = 3
key_vault_name  = "kv-amcart-staging-westeurope"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-staging-westeurope"
	}
	catalog = {
		name = "mi-amcart-catalog-staging-westeurope"
	}
	order = {
		name = "mi-amcart-order-staging-westeurope"
	}
	search = {
		name = "mi-amcart-search-staging-westeurope"
	}
}

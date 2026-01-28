location        = "westeurope"
resource_group  = "rg-amcart-dev-westeurope"
environment     = "dev"
region          = "westeurope"
aks_node_count  = 2
postgres_tier   = "GP_Standard_D2s_v3"
redis_capacity  = 1
kafka_capacity  = 2
key_vault_name  = "kv-amcart-dev-westeurope"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-dev-westeurope"
	}
	catalog = {
		name = "mi-amcart-catalog-dev-westeurope"
	}
	order = {
		name = "mi-amcart-order-dev-westeurope"
	}
	search = {
		name = "mi-amcart-search-dev-westeurope"
	}
}

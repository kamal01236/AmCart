location        = "westeurope"
resource_group  = "rg-amcart-prod-westeurope"
environment     = "prod"
region          = "westeurope"
aks_node_count  = 5
postgres_tier   = "GP_Standard_D8s_v3"
redis_capacity  = 3
kafka_capacity  = 5
key_vault_name  = "kv-amcart-prod-westeurope"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-prod-westeurope"
	}
	catalog = {
		name = "mi-amcart-catalog-prod-westeurope"
	}
	order = {
		name = "mi-amcart-order-prod-westeurope"
	}
	search = {
		name = "mi-amcart-search-prod-westeurope"
	}
}

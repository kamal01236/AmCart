location        = "eastus"
resource_group  = "rg-amcart-prod-eastus"
environment     = "prod"
region          = "eastus"
aks_node_count  = 5
postgres_tier   = "GP_Standard_D8s_v3"
redis_capacity  = 3
kafka_capacity  = 5
key_vault_name  = "kv-amcart-prod-eastus"
tags = {
	application = "amcart"
	cost_center = "ecommerce"
}

service_managed_identities = {
	auth = {
		name = "mi-amcart-auth-prod-eastus"
	}
	catalog = {
		name = "mi-amcart-catalog-prod-eastus"
	}
	order = {
		name = "mi-amcart-order-prod-eastus"
	}
	search = {
		name = "mi-amcart-search-prod-eastus"
	}
}

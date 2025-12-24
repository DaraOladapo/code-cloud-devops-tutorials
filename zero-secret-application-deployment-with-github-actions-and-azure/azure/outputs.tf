# Output values for ToDo App infrastructure

output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "resource_group_location" {
  value = azurerm_resource_group.main.location
}

output "managed_identity_client_id" {
  value = module.managed_identity.client_id
}

output "managed_identity_principal_id" {
  value       = module.managed_identity.principal_id
  description = "Principal ID of the web app's User Assigned Managed Identity"
}

output "managed_identity_name" {
  value = var.managed_identity_name
  description = "Name of the managed identity for database access configuration"
}

output "key_vault_uri" {
  value = module.key_vault.vault_uri
}

output "sql_server_name" {
  value = module.sql_server.server_name
  description = "Name of the SQL Server (without .database.windows.net)"
}

output "sql_server_fqdn" {
  value = module.sql_server.fully_qualified_domain_name
}

output "sql_database_name" {
  value = module.sql_server.database_name
}

output "web_app_name" {
  value = module.app_service.app_service_name
  description = "Name of the Azure App Service"
}

output "web_app_url" {
  value = "https://${module.app_service.default_hostname}"
}

output "app_insights_connection_string" {
  value     = module.monitoring.connection_string
  sensitive = true
}

output "service_principal_object_id" {
  value       = data.azuread_service_principal.current.object_id
  description = "Object ID of the service principal used for SQL Server admin"
}

output "tenant_id" {
  value       = data.azurerm_client_config.current.tenant_id
  description = "Azure AD Tenant ID"
}

output "sql_server_identity_principal_id" {
  value       = module.sql_server.identity_principal_id
  description = "Principal ID of SQL Server's System Assigned Managed Identity (needs Directory Reader role)"
}
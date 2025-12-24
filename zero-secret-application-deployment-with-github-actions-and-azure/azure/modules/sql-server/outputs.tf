# Outputs for SQL Server module

output "server_name" {
  value = azurerm_mssql_server.this.name
}

output "database_name" {
  value = azurerm_mssql_database.this.name
}

output "fully_qualified_domain_name" {
  value = azurerm_mssql_server.this.fully_qualified_domain_name
}

output "id" {
  value = azurerm_mssql_server.this.id
}

output "connection_string" {
  value     = "Server=${azurerm_mssql_server.this.fully_qualified_domain_name};Database=${azurerm_mssql_database.this.name};Authentication=Active Directory Default;"
  sensitive = true
}

output "identity_principal_id" {
  value       = azurerm_mssql_server.this.identity[0].principal_id
  description = "Principal ID of the SQL Server's System Assigned Managed Identity"
}

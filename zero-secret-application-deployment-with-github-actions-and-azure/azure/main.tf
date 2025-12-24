# Root Terraform configuration for ToDo App infrastructure

# Get current client configuration (service principal running Terraform)
data "azurerm_client_config" "current" {}

# Get service principal details for SQL Server admin
data "azuread_service_principal" "current" {
  client_id = data.azurerm_client_config.current.client_id
}

# Create the resource group first
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

module "managed_identity" {
  source              = "./modules/managed-identity"
  name                = var.managed_identity_name
  location            = var.location
  resource_group_name = azurerm_resource_group.main.name
  tags                = var.tags
}

module "key_vault" {
  source                        = "./modules/key-vault"
  name                          = var.key_vault_name
  location                      = var.location
  resource_group_name           = azurerm_resource_group.main.name
  tenant_id                     = data.azurerm_client_config.current.tenant_id
  sku_name                      = "standard"
  public_network_access_enabled = false
  tags                          = var.tags
}

module "sql_server" {
  source                        = "./modules/sql-server"
  name                          = var.sql_server_name
  resource_group_name           = azurerm_resource_group.main.name
  location                      = var.location
  entra_admin_login             = data.azuread_service_principal.current.display_name
  entra_admin_object_id         = data.azuread_service_principal.current.object_id
  public_network_access_enabled = true
  db_name                       = var.sql_db_name
  sku_name                      = var.sql_sku_name
  tags                          = var.tags
}

module "app_service" {
  source               = "./modules/app-service"
  plan_name            = var.app_service_plan_name
  app_name             = var.web_app_name
  location             = var.location
  resource_group_name  = azurerm_resource_group.main.name
  sku_name             = var.app_service_sku
  dotnet_version       = var.dotnet_version
  managed_identity_ids = [module.managed_identity.id]
  app_settings = {
    # Application Insights Configuration
    "APPLICATIONINSIGHTS_CONNECTION_STRING"   = module.monitoring.connection_string
    "APPINSIGHTS_INSTRUMENTATIONKEY"          = module.monitoring.instrumentation_key
    "ApplicationInsights__ConnectionString"   = module.monitoring.connection_string
    "ApplicationInsights__InstrumentationKey" = module.monitoring.instrumentation_key

    # Azure Services Configuration
    "KEY_VAULT_URI"              = module.key_vault.vault_uri
    "SQL_SERVER_FQDN"            = module.sql_server.fully_qualified_domain_name
    "SQL_DATABASE_NAME"          = module.sql_server.database_name
    "MANAGED_IDENTITY_CLIENT_ID" = module.managed_identity.client_id
    "AZURE_CLIENT_ID"            = module.managed_identity.client_id

    # Database Connection Strings
    "AZURE_SQL_CONNECTION_STRING"          = module.sql_server.connection_string
    "ConnectionStrings__DefaultConnection" = module.sql_server.connection_string
  }
  tags = var.tags
}

module "monitoring" {
  source              = "./modules/monitoring"
  name                = var.app_insights_name
  location            = var.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  tags                = var.tags
}

# Role assignments for SQL Database access
module "sql_role_assignments" {
  source               = "./modules/role-assignments"
  principal_id         = module.managed_identity.principal_id
  role_definition_name = "SQL DB Contributor"
  scope                = module.sql_server.id
}

# Role assignments for Key Vault access
module "keyvault_role_assignments" {
  source               = "./modules/role-assignments"
  principal_id         = module.managed_identity.principal_id
  role_definition_name = "Key Vault Secrets User"
  scope                = module.key_vault.id
}

# Role assignments for monitoring
module "monitoring_role_assignments" {
  source               = "./modules/role-assignments"
  principal_id         = module.managed_identity.principal_id
  role_definition_name = "Monitoring Metrics Publisher"
  scope                = module.monitoring.id
}

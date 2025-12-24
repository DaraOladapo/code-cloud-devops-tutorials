# Input variables for ToDo App infrastructure

variable "resource_group_name" {
  description = "Resource group name for all resources"
  type        = string
}

variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "UK South"
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default     = {}
}

# Managed Identity
variable "managed_identity_name" {
  description = "Name for the managed identity"
  type        = string
  default     = "todoapp-identity"
}

# Key Vault
variable "key_vault_name" {
  description = "Name for the Key Vault"
  type        = string
  default     = "todoappkvgwl"
}

# SQL Server
variable "sql_server_name" {
  description = "Name for the SQL Server"
  type        = string
  default     = "todoappsqlsrvgwl"
}
variable "sql_db_name" {
  description = "Name for the SQL Database"
  type        = string
  default     = "todoappdb"
}
variable "sql_sku_name" {
  description = "SKU for the SQL Database"
  type        = string
  default     = "Basic"
}

# App Service
variable "app_service_plan_name" {
  description = "Name for the App Service Plan"
  type        = string
  default     = "todoapp-asp"
}
variable "web_app_name" {
  description = "Name for the Web App"
  type        = string
  default     = "todoapp-webgwl"
}
variable "app_service_sku" {
  description = "SKU for the App Service Plan"
  type        = string
  default     = "B1"
}
variable "dotnet_version" {
  description = ".NET version for the Web App"
  type        = string
  default     = "8.0"
}

# Monitoring
variable "app_insights_name" {
  description = "Name for Application Insights"
  type        = string
  default     = "todoapp-ai"
}


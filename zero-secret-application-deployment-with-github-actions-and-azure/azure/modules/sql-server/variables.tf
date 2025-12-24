# Variables for SQL Server module

variable "name" {
  description = "SQL Server name"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "entra_admin_login" {
  description = "Entra ID admin login (email)"
  type        = string
}

variable "entra_admin_object_id" {
  description = "Entra ID admin object ID"
  type        = string
}

variable "public_network_access_enabled" {
  description = "Enable public network access"
  type        = bool
  default     = false
}

variable "db_name" {
  description = "Database name"
  type        = string
}

variable "sku_name" {
  description = "Database SKU name"
  type        = string
  default     = "Basic"
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
  default     = {}
}
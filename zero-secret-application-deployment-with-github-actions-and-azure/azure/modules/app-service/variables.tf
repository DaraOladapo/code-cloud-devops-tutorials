# Variables for App Service module

variable "plan_name" {
  description = "App Service Plan name"
  type        = string
}

variable "app_name" {
  description = "Web App name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "sku_name" {
  description = "App Service Plan SKU"
  type        = string
  default     = "B1"
}

variable "dotnet_version" {
  description = ".NET version for the app"
  type        = string
  default     = "8.0"
}

variable "app_settings" {
  description = "App settings for the web app"
  type        = map(string)
  default     = {}
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
  default     = {}
}

variable "managed_identity_ids" {
  description = "List of Managed Identity IDs to assign to the App Service"
  type        = list(string)
  default     = []
}

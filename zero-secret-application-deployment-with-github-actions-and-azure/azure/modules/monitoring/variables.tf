# Variables for Monitoring module

variable "name" {
  description = "Application Insights name"
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

variable "application_type" {
  description = "Type of Application Insights (web, other)"
  type        = string
  default     = "web"
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
  default     = {}
}

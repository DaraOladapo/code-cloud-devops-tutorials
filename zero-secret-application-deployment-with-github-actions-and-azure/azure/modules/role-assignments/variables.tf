# Variables for Role Assignments module

variable "principal_id" {
  description = "The principal ID to assign the role to"
  type        = string
}

variable "role_definition_name" {
  description = "The name of the role definition (e.g. 'Contributor', 'Reader')"
  type        = string
}

variable "scope" {
  description = "The scope at which the role assignment applies"
  type        = string
}

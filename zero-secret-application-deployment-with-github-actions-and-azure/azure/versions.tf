terraform {
  required_version = ">= 1.5.0"

  backend "azurerm" {
    resource_group_name  = "daraoladapo-central"  # Update with your Terraform state storage resource group
    storage_account_name = "docentralsa"  # Update with your Terraform state storage account
    container_name       = "terraform"
    key                  = "todoapp.tfstate"
    use_azuread_auth     = true
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.100"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.47"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.5"
    }
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {
}
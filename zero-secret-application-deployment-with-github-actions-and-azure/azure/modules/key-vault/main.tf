# Key Vault module

resource "azurerm_key_vault" "this" {
  name                          = var.name
  location                      = var.location
  resource_group_name           = var.resource_group_name
  tenant_id                     = var.tenant_id
  sku_name                      = var.sku_name
  purge_protection_enabled      = true
  public_network_access_enabled = var.public_network_access_enabled
  enable_rbac_authorization     = true # Use RBAC for modern access control
  tags                          = var.tags
}

# App Service module

resource "azurerm_service_plan" "this" {
  name                = var.plan_name
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = var.sku_name
  tags                = var.tags
}

resource "azurerm_linux_web_app" "this" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.this.id
  tags                = var.tags

  # Enable Managed Identity for secure Azure service access
  identity {
    type         = "UserAssigned"
    identity_ids = var.managed_identity_ids
  }

  site_config {
    always_on = var.sku_name != "F1" && var.sku_name != "D1" ? true : false
    application_stack {
      dotnet_version = var.dotnet_version
    }
  }

  app_settings = var.app_settings
}

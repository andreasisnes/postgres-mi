terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.112.0"
    }
  }
}

data "azurerm_subnet" "name" {
  resource_group_name  = "rgshared001at21"
  name                 = "default"
  virtual_network_name = "vnetshared001at21"

}

resource "azurerm_resource_group" "rg" {
  name     = "rgandreastest"
  location = "norwayeast"
}

resource "azurerm_linux_web_app" "example" {
  name                      = "webappandreastest"
  resource_group_name       = azurerm_resource_group.rg.name
  location                  = azurerm_resource_group.rg.location
  service_plan_id           = azurerm_service_plan.plan.id
  virtual_network_subnet_id = data.azurerm_subnet.name.id

  app_settings = {

  }

  site_config {

  }
}

resource "azurerm_service_plan" "plan" {
  name                = "apspandreastest"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_app_service_source_control" "source_control" {
  app_id   = azurerm_service_plan.plan.id
  repo_url = "https://github.com/andreasisnes/servicebus-mi/microservices/Microservice.HelloCache"
  branch   = "main"
}

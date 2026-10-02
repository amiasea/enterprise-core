data "azurerm_client_config" "current" {}

data "azuread_service_principal" "current" {
  client_id = data.azurerm_client_config.current.client_id
}

data "azuread_directory_roles" "current" {}

data "azuread_group" "enterprise_sql_admins" {
  display_name = "Amiasea Enterprise SQL Admin - ${var.environment}"
}

data "azuread_group" "enterprise_sql_migrations" {
  display_name = "Amiasea Enterprise SQL Migration - ${var.environment}"
}

data "azurerm_key_vault" "imperative" {
  name                = "kv-amiasea-imperative"
  resource_group_name = "rg-amiasea-imperative"
}



resource "azurerm_mssql_server" "enterprise" {
  name                = "sql-amiasea-enterprise-${lower(var.environment)}"
  resource_group_name = azurerm_resource_group.enterprise.name
  location            = azurerm_resource_group.enterprise.location
  version             = "12.0"

  azuread_administrator {
    login_username              = data.azuread_service_principal.current.display_name
    object_id                   = data.azuread_service_principal.current.object_id
    tenant_id                   = data.azurerm_client_config.current.tenant_id
    azuread_authentication_only = true
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.enterprise.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azapi_resource" "amiasea_database" {
  type      = "Microsoft.Sql/servers/databases@2025-02-01-preview"
  name      = "amiasea-enterprise"
  parent_id = azurerm_mssql_server.enterprise.id
  location  = var.location

  response_export_values = ["name"]

  body = {
    sku = {
      name     = "GP_S_Gen5_2"
      tier     = "GeneralPurpose"
      family   = "Gen5"
      capacity = 2
    }

    properties = {
      collation                        = "SQL_Latin1_General_CP1_CI_AS"
      maxSizeBytes                     = 34359738368
      minCapacity                      = 0.5
      autoPauseDelay                   = 60
      requestedBackupStorageRedundancy = "Local"

      useFreeLimit                = true
      freeLimitExhaustionBehavior = "AutoPause"
    }
  }
}



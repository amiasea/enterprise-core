resource "azurerm_container_app_job" "sql_establishment" {
  name                         = "job-amiasea-enterprise-sql-establishment-${lower(var.environment)}"
  resource_group_name          = azurerm_resource_group.enterprise.name
  location                     = azurerm_resource_group.enterprise.location
  container_app_environment_id = azurerm_container_app_environment.enterprise.id

  identity {
    type = "SystemAssigned"
  }

  replica_timeout_in_seconds = 300
  replica_retry_limit        = 1

  manual_trigger_config {
    parallelism              = 1
    replica_completion_count = 1
  }

  template {
    container {
      name   = "sql-establishment"
      image  = "ghcr.io/amiasea/image-utility:v1.0.1"
      cpu    = 0.25
      memory = "0.5Gi"

      command = [
        "pwsh",
        "-NoLogo",
        "-NoProfile",
        "-NonInteractive",
        "-File",
        "/opt/amiasea/scripts/establish-sql-principals.ps1",
      ]

      args = [
        "-ServerInstance",
        azurerm_mssql_server.enterprise.fully_qualified_domain_name,
        "-Database",
        azapi_resource.amiasea_database.name,
        "-AdminGroup",
        data.azuread_group.enterprise_sql_admins.display_name,
        "-AdminObjectId",
        data.azuread_group.enterprise_sql_admins.object_id,
        "-MigrationGroup",
        data.azuread_group.enterprise_sql_migrations.display_name,
        "-MigrationObjectId",
        data.azuread_group.enterprise_sql_migrations.object_id,
      ]
    }
  }

  secret {
    name                = "ghcr-app-token"
    identity            = "System"
    key_vault_secret_id = "${data.azurerm_key_vault.imperative.id}/secrets/amiasea-github-pat"
  }

  registry {
    server               = "ghcr.io"
    username             = var.pat_user_name
    password_secret_name = "ghcr-app-token"
  }

  depends_on = [
    azapi_resource.amiasea_database,
    azuread_directory_role_assignment.sql_server_directory_readers,
  ]
}

resource "azurerm_role_assignment" "sql_establishment_key_vault_secrets_user" {
  scope                = data.azurerm_key_vault.imperative.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_container_app_job.sql_establishment.identity[0].principal_id
}

resource "azapi_resource_action" "run_sql_establishment" {
  type        = "Microsoft.App/jobs@2026-07-01"
  resource_id = azurerm_container_app_job.sql_establishment.id
  action      = "start"

  response_export_values = ["*"]

  depends_on = [
    azurerm_container_app_job.sql_establishment,
  ]
}
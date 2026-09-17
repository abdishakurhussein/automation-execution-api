resource "azurerm_user_assigned_identity" "github_actions" {
  name                = "id-github-automation-execution-dev"
  location            = data.azurerm_resource_group.existing.location
  resource_group_name = data.azurerm_resource_group.existing.name
}

resource "azurerm_federated_identity_credential" "github_master" {
  name = "github-master"

  # Attach this trust rule to GitHub's deployment identity.
  user_assigned_identity_id = azurerm_user_assigned_identity.github_actions.id

  audience = ["api://AzureADTokenExchange"]
  issuer   = "https://token.actions.githubusercontent.com"

  # Match GitHub's exact subject, including owner/repository IDs and master.
  subject = "repo:abdishakurhussein@248567315/automation-execution-api@1361872131:ref:refs/heads/master"
}

resource "azurerm_role_assignment" "github_acr_push" {
  # Limit registry permissions to this ACR.
  scope                = data.azurerm_container_registry.existing.id
  role_definition_name = "AcrPush"
  principal_id         = azurerm_user_assigned_identity.github_actions.principal_id
}

resource "azurerm_role_assignment" "github_container_app" {
  # Allow deployment management only on this Container App.
  scope                = azurerm_container_app.api.id
  role_definition_name = "Container Apps Contributor"
  principal_id         = azurerm_user_assigned_identity.github_actions.principal_id
}

resource "azurerm_role_assignment" "github_runtime_identity" {
  # Allow the deployment to use the app's existing runtime identity.
  scope                = azurerm_user_assigned_identity.container_app.id
  role_definition_name = "Managed Identity Operator"
  principal_id         = azurerm_user_assigned_identity.github_actions.principal_id
}
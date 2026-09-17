output "container_app_url" {
  description = "Stable public HTTPS URL for the API"

  # Use the application's address instead of a revision-specific address.
  value = "https://${azurerm_container_app.api.ingress[0].fqdn}"
}

output "container_app_name" {
  value = azurerm_container_app.api.name
}

output "github_actions_client_id" {
  description = "Client ID used by GitHub Actions to sign in to Azure"
  value       = azurerm_user_assigned_identity.github_actions.client_id
}

output "github_actions_tenant_id" {
  description = "Azure tenant containing the GitHub deployment identity"
  value       = azurerm_user_assigned_identity.github_actions.tenant_id
} 
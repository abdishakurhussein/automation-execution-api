output "container_app_url" {
  description = "Public HTTPS URL for the API"
  value       = "https://${azurerm_container_app.api.latest_revision_fqdn}"
}

output "container_app_name" {
  value = azurerm_container_app.api.name
}
variable "subscription_id" {
  description = "Azure subscription ID"
  type        = string
  sensitive   = true
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "UK South"
}

variable "resource_group_name" {
  description = "Existing Azure resource group"
  type        = string
  default     = "rg-automation-execution-dev"
}

variable "container_registry_name" {
  description = "Existing Azure Container Registry"
  type        = string
  default     = "abdiautomationregistry"
}
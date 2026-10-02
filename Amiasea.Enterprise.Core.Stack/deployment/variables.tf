variable "environment" {
  type = string

  validation {
    condition     = contains(["dev", "prod"], var.environment)
    error_message = "Environment must be one of: dev, prod."
  }
}

variable "location" {
  type = string
  default = "centralus"
}

variable "identity_token" {
  type     = string
  ephemeral = true
}

variable "pat_user_name" {
  type = string
  default = "AlfredoBall"
}
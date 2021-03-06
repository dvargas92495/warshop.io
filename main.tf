terraform {
  backend "remote" {
    hostname = "app.terraform.io"
    organization = "VargasArts"
    workspaces {
      prefix = "warshop-io"
    }
  }
  required_providers {
    github = {
      source = "integrations/github"
      version = "4.2.0"
    }
  }
}

variable "aws_access_token" {
  type = string
}

variable "aws_secret_token" {
  type = string
}

variable "github_token" {
  type = string
}

variable "secret" {
  type = string
}

variable "clerk_api_key" {
    type = string
}

variable "mysql_password" {
  type = string
}

provider "aws" {
  region = "us-east-1"
  access_key = var.aws_access_token
  secret_key = var.aws_secret_token
}

provider "github" {
  owner = "dvargas92495"
  token = var.github_token
}

module "aws_static_site" {
  source  = "dvargas92495/static-site/aws"
  version = "3.6.2"

  origin_memory_size = 5120
  origin_timeout = 20
  domain = "warshop.io"
  secret = var.secret
  tags = {
      Application = "warshop-io"
  }

  providers = {
    aws.us-east-1 = aws
  }
}

module "aws_clerk" {
  source   = "dvargas92495/clerk/aws"
  version  = "1.0.4"

  zone_id  = module.aws_static_site.route53_zone_id
  clerk_id = "ll3czywatvsp"
}

module "aws-serverless-backend" {
    source  = "dvargas92495/serverless-backend/aws"
    version = "2.5.0"

    api_name = "warshop-io"
    directory = "api"
}

resource "github_actions_secret" "deploy_aws_access_key" {
  repository       = "warshop.io"
  secret_name      = "DEPLOY_AWS_ACCESS_KEY"
  plaintext_value  = module.aws_static_site.deploy-id
}

resource "github_actions_secret" "deploy_aws_access_secret" {
  repository       = "warshop.io"
  secret_name      = "DEPLOY_AWS_ACCESS_SECRET"
  plaintext_value  = module.aws_static_site.deploy-secret
}
resource "github_actions_secret" "lambda_aws_access_key" {
  repository       = "warshop.io"
  secret_name      = "LAMBDA_AWS_ACCESS_KEY"
  plaintext_value  = module.aws-serverless-backend.access_key
}

resource "github_actions_secret" "lambda_aws_access_secret" {
  repository       = "warshop.io"
  secret_name      = "LAMBDA_AWS_ACCESS_SECRET"
  plaintext_value  = module.aws-serverless-backend.secret_key
}

resource "github_actions_secret" "clerk_api_key" {
  repository       = "warshop.io"
  secret_name      = "CLERK_API_KEY"
  plaintext_value  = var.clerk_api_key
}

resource "github_actions_secret" "mysql_password" {
  repository       = "warshop.io"
  secret_name      = "MYSQL_PASSWORD"
  plaintext_value  = var.mysql_password
}

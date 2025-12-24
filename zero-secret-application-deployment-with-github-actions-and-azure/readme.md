# Zero-Secret Application Deployment with GitHub Actions and Azure


A complete template demonstrating **passwordless, zero-secret deployment** using GitHub Actions, Azure Managed Identity, and OpenID Connect (OIDC). Deploy a .NET ToDo application to Azure without managing any secrets or passwords in your code or CI/CD pipeline.

## What This Demonstrates

- **Zero Secrets** - No passwords, connection strings, or API keys anywhere
- **OIDC Authentication** - GitHub Actions authenticate to Azure using OpenID Connect  
- **Managed Identity** - Application uses Azure Managed Identity for all service access
- **Infrastructure as Code** - Complete infrastructure provisioned with Terraform
- **Automated CI/CD** - Full pipeline with GitHub Actions
- **Azure SQL with AAD** - SQL Server authentication using Azure Active Directory
- **Application Insights** - Built-in monitoring and telemetry

## Architecture

```
GitHub Actions (OIDC) ──▶ Azure Federated Identity
    │                           │
    ├─ Build .NET App          │
    ├─ Provision Infra ────────┘
    └─ Deploy to Azure
           │
           └──▶ App Service (Managed Identity)
                    ├──▶ Azure SQL (AAD Auth)
                    ├──▶ Key Vault (RBAC)
                    └──▶ Application Insights
```

## Project Structure

```
.
├── .github/workflows/
│   ├── deploy-todo.yml.example     # GitHub Actions workflow template
│   └── deploy-todo.yml             # Your workflow (gitignored)
├── azure/
│   ├── main.tf                     # Root Terraform configuration
│   ├── variables.tf                # Input variables
│   ├── terraform.tfvars.example    # Variable template
│   ├── terraform.tfvars            # Your values (gitignored)
│   ├── modules/                    # Terraform modules
│   │   ├── app-service/
│   │   ├── key-vault/
│   │   ├── managed-identity/
│   │   ├── monitoring/
│   │   ├── role-assignments/
│   │   └── sql-server/
│   └── scripts/
│       ├── add-users-to-db.sql
│       └── grant-directory-reader.ps1
└── src/ToDoApp/                    # .NET 8.0 MVC Application
```

## Prerequisites

- Azure subscription with Contributor access
- GitHub account
- Azure CLI installed
- Terraform 1.5+ installed
- .NET 8.0 SDK (for local development)
- PowerShell (for setup scripts)

## Quick Start

### Step 1: Clone and Setup Azure AD

```bash
# Clone repository
git clone https://github.com/YOUR-USERNAME/YOUR-REPO.git
cd YOUR-REPO

# Login to Azure
az login
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Create Azure AD application for GitHub OIDC
APP_NAME="github-actions-oidc-app"
GITHUB_ORG="your-github-username"
REPO_NAME="your-repo-name"

APP_ID=$(az ad app create --display-name "$APP_NAME" --query appId -o tsv)
az ad sp create --id $APP_ID

# Configure federated credentials for OIDC
az ad app federated-credential create --id $APP_ID --parameters "{
  \"name\": \"github-actions-main\",
  \"issuer\": \"https://token.actions.githubusercontent.com\",
  \"subject\": \"repo:$GITHUB_ORG/$REPO_NAME:ref:refs/heads/main\",
  \"audiences\": [\"api://AzureADTokenExchange\"]
}"

# Assign Azure roles
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
az role assignment create --assignee $APP_ID --role "Contributor" --scope "/subscriptions/$SUBSCRIPTION_ID"
az role assignment create --assignee $APP_ID --role "User Access Administrator" --scope "/subscriptions/$SUBSCRIPTION_ID"

# Note these values
TENANT_ID=$(az account show --query tenantId -o tsv)
echo "Application ID: $APP_ID"
echo "Tenant ID: $TENANT_ID"
echo "Subscription ID: $SUBSCRIPTION_ID"
```

### Step 2: Configure GitHub Repository

Navigate to: **Settings → Secrets and variables → Actions → Variables**

Add these three **variables** (NOT secrets):
- `AZURE_CLIENT_ID` = Application ID from above
- `AZURE_TENANT_ID` = Tenant ID from above  
- `AZURE_SUBSCRIPTION_ID` = Subscription ID from above

### Step 3: Configure Terraform

```bash
# Copy and edit Terraform variables
cd azure
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your values (see below)

# Update backend configuration in versions.tf
# Match these values in your workflow file
```

**terraform.tfvars** - Update these values (must be globally unique):

```hcl
resource_group_name = "my-todo-app-rg"
location            = "UK South"

# MUST BE GLOBALLY UNIQUE
key_vault_name      = "my-todo-kv-12345"      # 3-24 chars, alphanumeric + hyphens
sql_server_name     = "my-todo-sql-12345"     # lowercase, alphanumeric + hyphens  
web_app_name        = "my-todo-app-12345"     # alphanumeric + hyphens

managed_identity_name = "my-todo-identity"
sql_db_name           = "TodoDatabase"
sql_sku_name          = "Basic"               # Basic, S0, S1, P1
app_service_plan_name = "my-todo-asp"
app_service_sku       = "B1"                  # F1, B1, S1, P1V2
dotnet_version        = "8.0"
app_insights_name     = "my-todo-ai"

tags = {
  environment = "dev"
  project     = "todo-app"
}
```

### Step 4: Configure GitHub Actions Workflow

```bash
# Copy and edit workflow
cd .github/workflows
cp deploy-todo.yml.example deploy-todo.yml
```

**deploy-todo.yml** - Update environment variables:

```yaml
env:
  TF_STATE_STORAGE_ACCOUNT: 'mytfstate12345'      # MUST BE GLOBALLY UNIQUE
  TF_STATE_CONTAINER: 'terraform'
  TF_STATE_RESOURCE_GROUP: 'my-tfstate-rg'
  TF_STATE_LOCATION: 'uksouth'
  TF_STATE_SUBSCRIPTION_ID: 'your-subscription-id'
```

**versions.tf** - Update backend (must match workflow values):

```hcl
backend "azurerm" {
  resource_group_name  = "my-tfstate-rg"           # Match workflow
  storage_account_name = "mytfstate12345"          # Match workflow
  container_name       = "terraform"
  key                  = "todo-app.tfstate"
}
```

### Step 5: Deploy

```bash
# Commit and push to trigger deployment
git add .
git commit -m "Initial deployment configuration"
git push origin main
```

The GitHub Actions workflow will:
1. Build the .NET application
2. Create Terraform backend storage (if needed)
3. Provision Azure infrastructure
4. Deploy the application

Monitor progress in the **Actions** tab of your GitHub repository.

### Step 6: Post-Deployment (Optional)

Grant Directory Reader role for SQL Server AAD authentication (requires Global Admin):

```powershell
cd azure
pwsh scripts/grant-directory-reader.ps1 `
  -ServicePrincipalObjectId "SQL_SERVER_IDENTITY_ID" `
  -TenantId "YOUR_TENANT_ID"
```

Get your deployed app URL:

```bash
cd azure
terraform output web_app_url
```

## How It Works

### Zero-Secret Authentication

**Traditional approach (❌):**
```yaml
# Storing secrets in GitHub
env:
  AZURE_CLIENT_SECRET: ${{ secrets.AZURE_CLIENT_SECRET }}
  SQL_PASSWORD: ${{ secrets.SQL_PASSWORD }}
```

**This project (✅):**
```yaml
# OIDC - no secrets needed
permissions:
  id-token: write

- uses: azure/login@v2
  with:
    client-id: ${{ vars.AZURE_CLIENT_ID }}        # Public values
    tenant-id: ${{ vars.AZURE_TENANT_ID }}        # Not secrets!
    subscription-id: ${{ vars.AZURE_SUBSCRIPTION_ID }}
```

**Application code:**
```csharp
// No passwords in connection strings
var connectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
// Format: Server=...;Database=...;Authentication=Active Directory Default;
```

### Security Features

- **GitHub → Azure**: Uses OIDC federation (no service principal passwords)
- **App → SQL**: Uses Managed Identity with AAD authentication
- **App → Key Vault**: Uses Managed Identity for secret access
- **App → App Insights**: Uses Managed Identity for telemetry

## Local Development

```bash
cd src/ToDoApp

# Set up local database
dotnet ef database update

# Run the application
dotnet run
```

Access at: `https://localhost:5001`

## Customization

### Change App Service SKU

Edit `azure/terraform.tfvars`:
```hcl
app_service_sku = "B1"  # F1 (Free), B1 ($13/mo), S1 ($69/mo), P1V2 ($146/mo)
```

### Add New Azure Resources

1. Create module in `azure/modules/your-resource/`
2. Reference in `azure/main.tf`
3. Add role assignments if needed
4. Update outputs

## Monitoring

### View Logs

```bash
# Stream App Service logs
az webapp log tail --name YOUR_WEB_APP_NAME --resource-group YOUR_RG_NAME

# View in Application Insights
# Azure Portal → App Insights → Logs
# Query: traces | where timestamp > ago(1h)
```

### Application Insights Features
- **Live Metrics** - Real-time performance
- **Failures** - Exception tracking
- **Performance** - Request durations
- **Logs** - Custom queries

## Troubleshooting

### OIDC Authentication Failed
- Verify GitHub variables are set correctly (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`)
- Ensure federated credentials configured for your repository
- Check service principal has required roles

### Terraform Backend Not Found
- Workflow automatically creates storage account
- Ensure `TF_STATE_SUBSCRIPTION_ID` is correct
- Verify service principal has Contributor access

### SQL Connection Failed
- Check Azure Services firewall rule enabled
- Verify managed identity has database permissions
- Ensure Directory Reader role granted (if using AAD)

### Resource Names Already Exist
- Resource names must be globally unique
- Append random numbers: `my-app-12345`
- Update in `terraform.tfvars`

### Key Vault Access Denied
- Verify managed identity has "Key Vault Secrets User" role
- Check Key Vault firewall settings
- Ensure not restricted to selected networks

## Technology Stack

- **Application**: .NET 8.0 (ASP.NET Core MVC)
- **Database**: Azure SQL with Entity Framework Core
- **Infrastructure**: Terraform (modular)
- **CI/CD**: GitHub Actions with OIDC
- **Authentication**: Azure AD, Managed Identity
- **Monitoring**: Application Insights

## Important Files

### Files to Create (from .example templates)

1. **azure/terraform.tfvars** (copy from terraform.tfvars.example)
   - All resource configuration
   - Must update globally unique names

2. **.github/workflows/deploy-todo.yml** (copy from deploy-todo.yml.example)
   - GitHub Actions workflow
   - Update backend configuration

### Files NOT to Commit (gitignored)

- `azure/terraform.tfvars` - Your specific configuration
- `.github/workflows/deploy-todo.yml` - Your workflow config
- `azure/.terraform/` - Terraform cache
- `azure/*.tfstate*` - Terraform state

## Cost Estimates (Basic tier)

- App Service (B1): ~$13/month
- Azure SQL (Basic): ~$5/month
- Application Insights: Free tier available
- Storage (state): ~$0.02/month
- **Total**: ~$18/month

Use F1 (Free) tier for App Service to stay under $5/month total.

## Security Best Practices

1. ✅ Never commit secrets - Uses OIDC, no secrets needed
2. ✅ Use Terraform state encryption - Enabled by default
3. ✅ Restrict Key Vault access - Managed identity only
4. ✅ Enable Managed Identity - No connection strings
5. ✅ Review role assignments - Principle of least privilege
6. ✅ Monitor with App Insights - Track all activity

## Learn More

- [Azure Managed Identity](https://learn.microsoft.com/azure/active-directory/managed-identities-azure-resources/)
- [GitHub OIDC with Azure](https://docs.github.com/actions/deployment/security-hardening-your-deployments/configuring-openid-connect-in-azure)
- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Azure App Service](https://learn.microsoft.com/azure/app-service/)

## Contributing

Contributions welcome! Areas for improvement:
- Add automated tests
- Implement deployment slots (blue/green)
- Add custom domain configuration
- Enhance monitoring with alerts
- Add database backup strategy
- Multi-environment support (dev/staging/prod)

## License

MIT License - see LICENSE file

---

**Need help?** Open an issue with details about your setup and the error you're encountering.


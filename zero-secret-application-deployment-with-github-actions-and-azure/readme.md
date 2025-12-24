# Zero-Secret Application Deployment with GitHub Actions and Azure

This project demonstrates passwordless deployment of a .NET 8.0 ToDo application to Azure using GitHub Actions OIDC, Managed Identity, and Terraform. No secrets or passwords are stored anywhere.

## Structure

- `src/ToDoApp/`: .NET 8.0 MVC application with Entity Framework Core
- `azure/`: Terraform modules for infrastructure provisioning
  - `modules/app-service/`: App Service and App Service Plan
  - `modules/sql-server/`: Azure SQL Server and Database with AAD auth
  - `modules/key-vault/`: Azure Key Vault for secrets management
  - `modules/managed-identity/`: User-assigned Managed Identity
  - `modules/role-assignments/`: RBAC role assignments
  - `modules/monitoring/`: Application Insights
- `.github/workflows/deploy-todo.yml.example`: GitHub Actions workflow template

## Prerequisites

- .NET 8.0 SDK
- Azure subscription with Contributor access
- Azure CLI installed and configured
- Terraform 1.5+ installed
- GitHub account
- PowerShell (for Azure AD setup scripts)

## How to Run

### Step 1: Create Azure AD Application for OIDC (via Azure Portal)

1. **Navigate to Azure Portal** → **Microsoft Entra ID** (formerly Azure Active Directory)

2. **Create App Registration:**
   - Go to **App registrations** → **New registration**
   - Name: `github-actions-oidc-app` (or your preferred name)
   - Supported account types: **Accounts in this organizational directory only**
   - Redirect URI: Leave blank
   - Click **Register**

3. **Note your Application (client) ID and Tenant ID:**
   - After registration, you'll see the **Overview** page
   - Copy and save: **Application (client) ID** → This is your `AZURE_CLIENT_ID`
   - Copy and save: **Directory (tenant) ID** → This is your `AZURE_TENANT_ID`

4. **Configure Federated Credentials for OIDC:**
   - In the app registration, go to **Certificates & secrets**
   - Select **Federated credentials** tab
   - Click **Add credential**
   - Select **GitHub Actions deploying Azure resources**
   - Fill in:
     - **Organization**: Your GitHub username or organization name
     - **Repository**: Your repository name
     - **Entity type**: Branch
     - **GitHub branch name**: `main`
     - **Name**: `github-actions-main`
   - Click **Add**

5. **Assign Azure Roles to the Service Principal:**
   - Go to **Subscriptions** → Select your subscription
   - Copy and save the **Subscription ID** → This is your `AZURE_SUBSCRIPTION_ID`
   - Go to **Access control (IAM)**
   - Click **Add** → **Add role assignment**
   - **Role**: Select **Contributor** → Click **Next**
   - **Assign access to**: User, group, or service principal
   - Click **Select members** → Search for `github-actions-oidc-app` → Select it
   - Click **Review + assign**
   - Repeat the same process to assign **User Access Administrator** role

6. **Save these three values** (you'll need them in Step 2):
   - `AZURE_CLIENT_ID`: Application (client) ID from step 3
   - `AZURE_TENANT_ID`: Directory (tenant) ID from step 3
   - `AZURE_SUBSCRIPTION_ID`: Subscription ID from step 5

<details>
<summary><strong>Alternative: Use Azure CLI Script</strong></summary>

If you prefer to use code instead of the Azure Portal, run this PowerShell script:

```pwsh
$appName = "github-actions-oidc-app"
$githubOrg = "YOUR-GITHUB-USERNAME"          # Change this
$repoName = "YOUR-REPO-NAME"                 # Change this

az login
az account set --subscription "YOUR_SUBSCRIPTION_ID"    # Change this

# Create app registration
$appId = az ad app create --display-name $appName --query appId -o tsv
az ad sp create --id $appId

# Configure OIDC federated credentials
az ad app federated-credential create --id $appId --parameters @"
{
  \"name\": \"github-actions-main\",
  \"issuer\": \"https://token.actions.githubusercontent.com\",
  \"subject\": \"repo:$githubOrg/${repoName}:ref:refs/heads/main\",
  \"audiences\": [\"api://AzureADTokenExchange\"]
}
"@

# Assign required Azure roles
$subscriptionId = az account show --query id -o tsv
az role assignment create --assignee $appId --role "Contributor" --scope "/subscriptions/$subscriptionId"
az role assignment create --assignee $appId --role "User Access Administrator" --scope "/subscriptions/$subscriptionId"

# Save these values - you'll need them
$tenantId = az account show --query tenantId -o tsv
Write-Host "`nSave these values:"
Write-Host "AZURE_CLIENT_ID: $appId"
Write-Host "AZURE_TENANT_ID: $tenantId"
Write-Host "AZURE_SUBSCRIPTION_ID: $subscriptionId"
```

</details>

### Step 2: Configure GitHub Repository

Go to your GitHub repository: **Settings → Secrets and variables → Actions → Variables**

Create these **repository variables** (not secrets):

- `AZURE_CLIENT_ID`: The Application ID from Step 1
- `AZURE_TENANT_ID`: The Tenant ID from Step 1
- `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID from Step 1

### Step 3: Configure Terraform Variables

Copy and edit the Terraform configuration file:

```pwsh
cd azure
cp terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars` with your specific values:

```hcl
# Required: Change all these values to be globally unique
resource_group_name   = "my-todo-app-rg"              # Change this
location              = "UK South"                     # Change if needed

# MUST BE GLOBALLY UNIQUE across all Azure
key_vault_name        = "my-todo-kv-12345"            # Change this (3-24 chars)
sql_server_name       = "my-todo-sql-12345"           # Change this (lowercase, alphanumeric, hyphens)
web_app_name          = "my-todo-app-12345"           # Change this (alphanumeric, hyphens)

# Optional: Adjust these based on your needs
managed_identity_name = "my-todo-identity"
sql_db_name           = "TodoDatabase"
sql_sku_name          = "Basic"                       # Basic (~$5/mo), S0, S1, P1
app_service_plan_name = "my-todo-asp"
app_service_sku       = "B1"                          # F1 (free), B1 (~$13/mo), S1, P1V2
dotnet_version        = "8.0"
app_insights_name     = "my-todo-ai"

tags = {
  environment = "dev"
  project     = "todo-app"
}
```

### Step 4: Configure Terraform Backend

Edit `azure/versions.tf` to set your Terraform state storage (must match workflow values):

```hcl
backend "azurerm" {
  resource_group_name  = "my-tfstate-rg"              # Change this
  storage_account_name = "mytfstate12345"             # Change this (globally unique)
  container_name       = "terraform"
  key                  = "todo-app.tfstate"
}
```

### Step 5: Configure GitHub Actions Workflow

Copy and edit the workflow file:

```pwsh
cd .github/workflows
cp deploy-todo.yml.example deploy-todo.yml
```

Edit `deploy-todo.yml` environment variables to match your backend config:

```yaml
env:
  TF_STATE_STORAGE_ACCOUNT: 'mytfstate12345'          # Must match versions.tf
  TF_STATE_CONTAINER: 'terraform'                     # Must match versions.tf
  TF_STATE_RESOURCE_GROUP: 'my-tfstate-rg'            # Must match versions.tf
  TF_STATE_LOCATION: 'uksouth'                        # Change if needed
  TF_STATE_SUBSCRIPTION_ID: 'your-subscription-id'    # Your subscription ID
```

### Step 6: Deploy to Azure

Commit and push to trigger the deployment:

```pwsh
git add .
git commit -m "Configure deployment for my environment"
git push origin main
```

The GitHub Actions workflow will automatically:

1. Create Terraform backend storage (if it doesn't exist)
2. Build the .NET application
3. Provision all Azure infrastructure with Terraform
4. Deploy the application to App Service

Monitor the deployment in your repository's **Actions** tab.

### Step 7: Access Your Application

Once deployment completes, get your application URL:

```pwsh
cd azure
terraform output web_app_url
```

Visit the URL to see your deployed ToDo application.

## Variables to Change

Before deploying, you must update these values:

### In Step 1 (PowerShell script)

- `$githubOrg`: Your GitHub username or organization name
- `$repoName`: Your repository name
- Subscription ID in `az account set` command

### In Step 3 (terraform.tfvars)

- `resource_group_name`: Your resource group name
- `location`: Your preferred Azure region
- `key_vault_name`: **Must be globally unique** (3-24 characters)
- `sql_server_name`: **Must be globally unique** (lowercase only)
- `web_app_name`: **Must be globally unique**
- `app_service_sku`: Choose based on your needs (F1=Free, B1=$13/mo, S1=$69/mo)
- `sql_sku_name`: Choose based on your needs (Basic=$5/mo, S0, S1, P1)

### In Step 4 (versions.tf)

- `resource_group_name`: Terraform state resource group name
- `storage_account_name`: **Must be globally unique** (lowercase, alphanumeric only)

### In Step 5 (deploy-todo.yml)

- `TF_STATE_STORAGE_ACCOUNT`: Must match `storage_account_name` from Step 4
- `TF_STATE_RESOURCE_GROUP`: Must match resource group from Step 4
- `TF_STATE_LOCATION`: Azure region for state storage
- `TF_STATE_SUBSCRIPTION_ID`: Your Azure subscription ID

## How It Works

This project uses **zero-secret authentication** - no passwords are stored anywhere:

- **GitHub Actions → Azure**: Uses OpenID Connect (OIDC) with federated credentials. GitHub proves its identity to Azure without needing a client secret.
- **Application → Azure SQL**: Uses Managed Identity with Azure AD authentication. No password in connection string.
- **Application → Key Vault**: Uses Managed Identity with RBAC. The app authenticates automatically.
- **Application → App Insights**: Uses Managed Identity for telemetry.

## Notes

- All infrastructure is provisioned automatically by Terraform using modular configuration
- The GitHub Actions workflow handles both infrastructure and application deployment
- Application uses Entity Framework Core migrations for database schema
- Terraform state is stored securely in Azure Storage
- The workflow creates the Terraform backend storage automatically if it doesn't exist
- All Azure services use the same Managed Identity for consistent authentication
- Cost estimate: ~$18/month with Basic SQL + B1 App Service, or ~$5/month with Basic SQL + F1 (free) App Service

> **Important:** Cloud platforms evolve rapidly, and you may encounter changes in the Azure Portal UI, API changes, or new requirements. If you run into any issues:
>
> - Drop a comment on the associated video describing your issue
> - Use the form in the repository home to ask questions
> - **Always check and follow the logs** in GitHub Actions (Actions tab) and Azure Portal for detailed error messages and troubleshooting guidance

---

This project follows Azure best practices for passwordless deployment and security. See the code and Terraform modules for implementation details.

---

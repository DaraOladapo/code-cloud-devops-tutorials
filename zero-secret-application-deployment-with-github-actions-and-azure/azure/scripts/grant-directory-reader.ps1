#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Grants Directory Reader role to the service principal (requires Global Admin)

.DESCRIPTION
    This script must be run by an Azure AD Global Administrator or Privileged Role Administrator.
    It grants the Directory Reader role to the GitHub Actions service principal, which is required
    for the SQL Server AD admin to resolve managed identities when creating database users.

.PARAMETER ServicePrincipalObjectId
    The Object ID of the service principal

.PARAMETER TenantId
    The Azure AD Tenant ID

.EXAMPLE
    .\grant-directory-reader.ps1 -ServicePrincipalObjectId "649707b4-cdbb-4926-aa06-44cbbe48c411" -TenantId "bfeb643b-0980-46ee-a765-5919c3e10ba9"

.NOTES
    Prerequisites:
    - Azure AD Global Administrator or Privileged Role Administrator role
    - Azure CLI installed and logged in (az login --tenant <tenant-id>)
    
    Only needs to be run once per service principal
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ServicePrincipalObjectId,
    
    [Parameter(Mandatory=$true)]
    [string]$TenantId
)

$ErrorActionPreference = "Stop"

Write-Host "📖 Granting Directory Reader role to service principal..." -ForegroundColor Cyan
Write-Host "   Tenant ID: $TenantId" -ForegroundColor Gray
Write-Host "   Service Principal Object ID: $ServicePrincipalObjectId" -ForegroundColor Gray

# Directory Reader role template ID (well-known GUID)
$DirectoryReaderRoleId = "88d8e3e3-8f55-4a1e-953a-9b9898b8876b"

try {
    # Check if role is already assigned
    Write-Host "`n🔍 Checking current role assignments..." -ForegroundColor Yellow
    
    $existingAssignment = az rest --method GET `
        --uri "https://graph.microsoft.com/v1.0/directoryRoles/roleTemplateId=$DirectoryReaderRoleId/members" `
        --query "value[?id=='$ServicePrincipalObjectId'].id" -o tsv
    
    if ($existingAssignment) {
        Write-Host "✅ Service principal already has Directory Reader role" -ForegroundColor Green
        exit 0
    }
    
    Write-Host "⚙️  Role not assigned yet, proceeding with assignment..." -ForegroundColor Yellow
    
    # Activate the Directory Reader role (if not already active) and get its ID
    Write-Host "`n📝 Activating Directory Reader role..." -ForegroundColor Yellow
    
    # Use cross-platform temp directory
    $tempDir = if ($IsLinux -or $IsMacOS) { "/tmp" } else { $env:TEMP }
    
    $activateJsonPath = Join-Path $tempDir "activate-role-$([guid]::NewGuid().ToString()).json"
    Set-Content -Path $activateJsonPath -Value "{`"roleTemplateId`":`"$DirectoryReaderRoleId`"}" -Encoding UTF8
    
    az rest --method POST `
        --uri "https://graph.microsoft.com/v1.0/directoryRoles" `
        --headers "Content-Type=application/json" `
        --body "@$activateJsonPath" `
        2>$null | Out-Null
    
    Remove-Item $activateJsonPath -Force -ErrorAction SilentlyContinue
    
    # Get the activated role's ID
    $roleId = az rest --method GET `
        --uri "https://graph.microsoft.com/v1.0/directoryRoles" `
        --query "value[?roleTemplateId=='$DirectoryReaderRoleId'].id | [0]" -o tsv
    
    if (-not $roleId) {
        throw "Failed to get Directory Reader role ID"
    }
    
    Write-Host "   Directory Reader role ID: $roleId" -ForegroundColor Gray
    
    # Assign the role to the service principal
    Write-Host "🔐 Assigning Directory Reader role to service principal..." -ForegroundColor Yellow
    
    $assignJsonPath = Join-Path $tempDir "assign-role-$([guid]::NewGuid().ToString()).json"
    Set-Content -Path $assignJsonPath -Value "{`"@odata.id`":`"https://graph.microsoft.com/v1.0/directoryObjects/$ServicePrincipalObjectId`"}" -Encoding UTF8
    
    az rest --method POST `
        --uri "https://graph.microsoft.com/v1.0/directoryRoles/$roleId/members/`$ref" `
        --headers "Content-Type=application/json" `
        --body "@$assignJsonPath"
    
    Remove-Item $assignJsonPath -Force -ErrorAction SilentlyContinue
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ Successfully granted Directory Reader role!" -ForegroundColor Green
        Write-Host "`n📋 Why this is needed:" -ForegroundColor Cyan
        Write-Host "   • SQL Server AD admin needs to read directory objects" -ForegroundColor Gray
        Write-Host "   • Required to resolve managed identities when creating database users" -ForegroundColor Gray
        Write-Host "   • Allows 'CREATE USER [identity] FROM EXTERNAL PROVIDER' to work" -ForegroundColor Gray
    } else {
        throw "Failed to assign Directory Reader role"
    }
    
} catch {
    Write-Error "Failed to grant Directory Reader role: $_"
    Write-Host "`n🔍 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "   1. Ensure you are logged in as Global Administrator or Privileged Role Administrator" -ForegroundColor Gray
    Write-Host "   2. Run: az login --tenant $TenantId" -ForegroundColor Gray
    Write-Host "   3. Verify service principal exists: az ad sp show --id $ServicePrincipalObjectId" -ForegroundColor Gray
    Write-Host "   4. Check your permissions: az ad signed-in-user show --query '{displayName:displayName, roles:appRoleAssignments}'" -ForegroundColor Gray
    exit 1
}

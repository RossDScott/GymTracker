// Hosts GymTracker.McpServer on Azure App Service (Linux) with a system-assigned managed identity
// that has blob data access to the existing backup container only.
//
// Deploy (one-off / when infra changes):
//   az deployment group create -g <app-resource-group> -f infra/mcp-server.bicep -p @infra/mcp-server.parameters.json

@description('Globally unique web app name; the MCP URL becomes https://<name>.azurewebsites.net/mcp')
param appName string

param location string = resourceGroup().location

@description('App Service plan SKU. B1 supports Always On, which avoids cold starts that can exceed Claude\'s 10s timeouts.')
@allowed(['F1', 'B1', 'B2', 'P0v3'])
param skuName string = 'B1'

@description('Auth0 tenant domain, e.g. my-tenant.eu.auth0.com')
param auth0Domain string

@description('Auth0 user id(s) allowed to use the server ("sub" claim), e.g. google-oauth2|1234')
param allowedSubjects array

@description('Name of the existing storage account holding the GymTracker backup blobs')
param storageAccountName string

@description('Resource group of the existing storage account')
param storageResourceGroup string = resourceGroup().name

@description('Name of the existing backup container')
param containerName string

var serverUrl = 'https://${appName}.azurewebsites.net/mcp'
var alwaysOn = skuName != 'F1'

var allowedSubjectSettings = [for (sub, i) in allowedSubjects: {
  name: 'Auth0__AllowedSubjects__${i}'
  value: sub
}]

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: '${appName}-plan'
  location: location
  kind: 'linux'
  sku: {
    name: skuName
  }
  properties: {
    reserved: true
  }
}

resource app 'Microsoft.Web/sites@2024-04-01' = {
  name: appName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: alwaysOn
      http20Enabled: true
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      ftpsState: 'Disabled'
      remoteDebuggingEnabled: false
      healthCheckPath: '/healthz'
      appSettings: concat([
        { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
        { name: 'Mcp__ServerUrl', value: serverUrl }
        { name: 'Auth0__Domain', value: auth0Domain }
        { name: 'Storage__BlobServiceUri', value: 'https://${storageAccountName}.blob.${environment().suffixes.storage}' }
        { name: 'Storage__ContainerName', value: containerName }
      ], allowedSubjectSettings)
    }
  }
}

// Deployments use Entra ID (GitHub OIDC), so username/password publishing credentials are switched off.
resource ftpCredentials 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  parent: app
  name: 'ftp'
  properties: {
    allow: false
  }
}

resource scmCredentials 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  parent: app
  name: 'scm'
  properties: {
    allow: false
  }
}

module storageAccess 'mcp-server-storage-role.bicep' = {
  name: '${appName}-storage-role'
  scope: resourceGroup(storageResourceGroup)
  params: {
    storageAccountName: storageAccountName
    containerName: containerName
    principalId: app.identity.principalId
  }
}

output mcpServerUrl string = serverUrl
output principalId string = app.identity.principalId

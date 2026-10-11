# GymTracker MCP Server

Remote [MCP](https://modelcontextprotocol.io) server exposing GymTracker data (exercises, plans, workouts, statistics) to Claude on web, desktop, mobile and Claude Code. It reads and writes the same Azure blobs the Blazor app backs up to.

## Security model

- **Transport:** Streamable HTTP at `https://<app>.azurewebsites.net/mcp`, stateless, HTTPS only (TLS 1.2+).
- **Authentication:** OAuth 2.1 per the [MCP authorization spec](https://modelcontextprotocol.io/specification/2025-11-25/basic/authorization). The server is a protected resource:
  - Unauthenticated calls get `401` with `WWW-Authenticate: Bearer resource_metadata=...`.
  - The metadata document (RFC 9728) names Auth0 as the authorization server.
  - Claude registers itself with Auth0 (Dynamic Client Registration), signs you in with PKCE, and sends a short-lived JWT access token.
- **Token validation:** issuer, signature (RS256 via Auth0 JWKS), lifetime, and audience. The audience must equal the MCP URL, so tokens minted for other APIs are rejected.
- **Authorization:** only Auth0 user ids listed in `Auth0:AllowedSubjects` are allowed (`403` otherwise). This matters because DCR lets anyone register a client.
- **Storage access:** the web app's managed identity has *Storage Blob Data Contributor* on the backup container only. There are no SAS tokens or keys.
- **Other hardening:**
  - Rate limiting: 120 requests/min per user.
  - 1 MB request body limit.
  - Storage error details are logged server-side and never returned to the client.
  - FTP and basic-auth publishing are disabled; deployment uses GitHub OIDC.
- **No secrets** in the repo or in App Service settings.

## One-time setup

### 1. Auth0 (free tier)
1. Create a tenant at auth0.com and note its domain, e.g. `gymtracker.eu.auth0.com`.
2. Under **Applications → APIs → Create API**:
   - **Identifier:** `https://<app>.azurewebsites.net/mcp`. This must exactly match the URL you'll add in Claude.
   - **Signing algorithm:** RS256.
   - In the API settings, enable **Allow Offline Access** (refresh tokens).
   - In the API settings, under **Default Permissions for Third-Party Applications**, set **User-Delegated Access** to **Authorized** and leave **Client Access** unauthorized.
     - Claude registers a new third-party client on each connection, so access can't be granted per application.
     - Without this, sign-in fails with `Client "..." is not authorized to access resource server "..."`.
3. Under **Settings**:
   - **General → API Authorization Settings → Default Audience:** the API identifier above.
   - **Advanced:** enable **Dynamic Client Registration**.
   - **Advanced:** enable the **Resource Parameter Compatibility Profile**, so Claude's RFC 8707 `resource` parameter is treated as the audience.
4. Under **Authentication → Database → Username-Password-Authentication**:
   - Enable **Promote Connection to Domain Level** so DCR-registered clients can use it.
   - Create your user under **User Management → Users → Create User**, then enable **Disable Sign Ups** on the connection.
   - A social login (e.g. Google) also works, but needs your own Google OAuth client; Auth0's shared dev keys aren't meant for real use.
5. Copy your **user_id** from **User Management → Users**, e.g. `auth0|65f1c…`. This is your `AllowedSubjects` value.
6. Recommended: under **Security → Multi-factor Auth**, enable **One-time Password** and set the policy to **Always**.
7. Optional defence in depth: add a Post-Login **Action** that calls `api.access.deny()` for any email other than yours.

### 2. Azure
1. Fill in `infra/mcp-server.parameters.json` (or pass `-p` values), then deploy:
   ```bash
   az group create -n rg-gymtracker-mcp -l uksouth
   az deployment group create -g rg-gymtracker-mcp -f infra/mcp-server.bicep -p @infra/mcp-server.parameters.json
   ```
   This creates the plan, the web app and its managed identity, and the container-scoped role assignment.
   - If it fails with `SubscriptionIsOverQuotaForSku`, the subscription has no App Service quota for that SKU in that region. Add `location=<region>` to deploy the app elsewhere, use `skuName=F1`, or request quota under **Quotas → App Service**.
   - F1 is free but sleeps when idle. The first request afterwards can take 10–20 s and may time out Claude's connection. Open `/healthz` to wake it, then retry.
2. Set up GitHub deployment without secrets (OIDC):
   - Create an Entra app registration.
   - Add a federated credential for this repo with entity type **Environment** and name `production`.
   - Grant it **Website Contributor** on the web app only.
   - In GitHub, add secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID` and `AZURE_SUBSCRIPTION_ID`, a variable `MCP_WEBAPP_NAME`, and an environment named `production`.
3. Push to `main` (or run the *Deploy MCP Server* workflow manually).
4. Check `https://<app>.azurewebsites.net/healthz` returns `Healthy`.

### 3. Claude
- **claude.ai / Desktop / mobile:**
  1. Go to **Settings → Connectors → Add custom connector**.
  2. Enter URL `https://<app>.azurewebsites.net/mcp` and leave the OAuth client ID/secret empty.
  3. Click **Connect** and sign in with Auth0.
  4. The connector syncs to the mobile app automatically.
- **Claude Code:** run `claude mcp add --transport http gymtracker https://<app>.azurewebsites.net/mcp`, then `/mcp` to sign in. Remove the old stdio registration (`claude mcp remove gymtracker`) first.

### Revoking access
Revoke your user's sessions/refresh tokens in Auth0, or remove the user id from the `Auth0__AllowedSubjects__N` app settings.

## Running locally

```bash
az login   # DefaultAzureCredential uses this; your account needs Storage Blob Data Contributor on the container
dotnet user-secrets --project src/GymTracker.McpServer set "Auth0:Domain" "<tenant>.auth0.com"
dotnet user-secrets --project src/GymTracker.McpServer set "Auth0:AllowedSubjects:0" "<your user id>"
dotnet user-secrets --project src/GymTracker.McpServer set "Storage:BlobServiceUri" "https://<account>.blob.core.windows.net"
dotnet user-secrets --project src/GymTracker.McpServer set "Storage:ContainerName" "<container>"
dotnet run --project src/GymTracker.McpServer
```

The local MCP URL is `http://localhost:5123/mcp`. To get tokens locally, add that URL as a second Auth0 API identifier and override `Mcp:ServerUrl` accordingly. Alternatively, test against the deployed instance with `npx @modelcontextprotocol/inspector`.

## Configuration reference

| Setting | Example |
|---|---|
| `Mcp:ServerUrl` | `https://gymtracker-mcp.azurewebsites.net/mcp` (resource id and required token audience) |
| `Auth0:Domain` | `gymtracker.eu.auth0.com` |
| `Auth0:AllowedSubjects:0..n` | `auth0\|65f1c0a1b2c3d4e5f6a7b8c9` |
| `Storage:BlobServiceUri` | `https://mystorage.blob.core.windows.net` |
| `Storage:ContainerName` | `backups` |

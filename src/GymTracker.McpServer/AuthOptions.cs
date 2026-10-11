namespace GymTracker.McpServer;

public class McpOptions
{
    public const string SectionName = "Mcp";

    /// <summary>
    /// Public URL of the MCP endpoint exactly as entered in Claude, e.g. https://myapp.azurewebsites.net/mcp.
    /// Used as the protected resource identifier and as the required access token audience.
    /// </summary>
    public string ServerUrl { get; set; } = "";
}

public class Auth0Options
{
    public const string SectionName = "Auth0";

    /// <summary>Auth0 tenant domain, e.g. my-tenant.eu.auth0.com</summary>
    public string Domain { get; set; } = "";

    /// <summary>Auth0 user ids ("sub" claim) allowed to use the server, e.g. google-oauth2|1234.</summary>
    public string[] AllowedSubjects { get; set; } = [];

    public string Issuer => $"https://{Domain}/";
}

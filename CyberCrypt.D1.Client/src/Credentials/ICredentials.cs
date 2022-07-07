namespace CyberCrypt.D1.Client.Credentials;

/// <summary>
/// Interface for credential implementations.
/// </summary>
public interface ICredentials
{
    /// <summary>
    /// Gets the token.
    /// </summary>
    string? GetToken();
    
    /// <inheritdoc cref="GetToken"/>
    Task<string?> GetTokenAsync();
}

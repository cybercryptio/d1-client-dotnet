namespace CyberCrypt.D1.Client.Credentials;

/// <summary>
/// Credentials based off a user provide token.
/// </summary>
public class TokenCredentials : ID1Credentials
{
    private readonly string? token;

    /// <summary>
    /// Initialize a new instance of the <see cref="TokenCredentials"/> class.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <returns>A new instance of the <see cref="TokenCredentials"/> class.</returns>
    public TokenCredentials(string token)
    {
        this.token = token;
    }

    /// <inheritdoc />
    public string? GetToken()
    {
        return token;
    }

    /// <inheritdoc />
    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(token);
    }
}
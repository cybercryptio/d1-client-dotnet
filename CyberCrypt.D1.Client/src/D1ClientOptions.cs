namespace CyberCrypt.D1.Client;

/// <summary>
/// Options for the <see cref="D1BaseClient" />.
/// </summary>
public class D1ClientOptions {
    /// <summary>
    /// The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The certificate path.
    /// </summary>
    public string? CertPath { get; set; }

    /// <summary>
    /// The access token to use.
    /// </summary>
    public string? AccessToken { get; set; }
}
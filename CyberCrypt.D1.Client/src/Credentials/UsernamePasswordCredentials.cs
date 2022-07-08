using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.Credentials;

/// <summary>
/// Credentials from username and password.
/// </summary>
public class UsernamePasswordCredentials : ID1Credentials
{
    private readonly Protobuf.Authn.AuthnClient client;
    private readonly string username;
    private readonly string password;
    private string? accessToken;

    /// <summary>
    /// The token expiration time.
    /// </summary>
    public DateTime ExpiryTime { get; internal set; } = DateTime.MinValue.AddMinutes(1); // Have to add one minute to avoid exception because of underflow when calculating if the token is expired.


    /// <summary>
    /// Initialize a new instance of the <see cref="D1BaseClient"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint of the D1 server.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="certPath">The certificate path.</param>
    /// <returns>A new instance of the <see cref="UsernamePasswordCredentials"/> class.</returns>
    public UsernamePasswordCredentials(string endpoint, string username, string password, string? certPath = null)
    {
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password)) {
            this.username = username;
            this.password = password;
        } else {
            throw new ArgumentNullException("username and password must be provided");
        }

        GrpcChannel channel;
        if (string.IsNullOrWhiteSpace(certPath))
        {
            channel = GrpcChannel.ForAddress(endpoint);
        }
        else
        {
            var cert = new X509Certificate2(File.ReadAllBytes(certPath));

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions { HttpHandler = handler });
        }

        client = new(channel);
    }

    /// <inheritdoc />
    public string? GetToken()
    {
        if (DateTime.Now > ExpiryTime.AddMinutes(-1)) {
            var req = client.LoginUser(new Protobuf.LoginUserRequest { UserId = username, Password = password });
            accessToken = req.AccessToken;
        }

        return accessToken;
    }

    /// <inheritdoc />
    public async Task<string?> GetTokenAsync()
    {
        if (DateTime.Now > ExpiryTime.AddMinutes(-1)) {
            var req = await client.LoginUserAsync(new Protobuf.LoginUserRequest { UserId = username, Password = password }).ConfigureAwait(false);
            accessToken = req.AccessToken;
        }

        return accessToken;
    }
}

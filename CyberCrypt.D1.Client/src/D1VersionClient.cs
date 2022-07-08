using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.Response;
using Grpc.Core;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Version client
/// </summary>
public interface ID1VersionClient
{
    /// <summary>
    /// Get the version of the D1 server.
    /// </summary>
    /// <returns>An instance of <see cref="VersionResponse"/>.</returns>
    VersionResponse Version();

    /// <inheritdoc cref="Version"/>
    Task<VersionResponse> VersionAsync();
}

/// <summary>
/// Version client for connection to a D1 server.
/// </summary>
public class D1VersionClient : ID1VersionClient
{
    private readonly Protobuf.Version.VersionClient client;
    private readonly ID1Credentials credentials;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1BaseClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1VersionClient"/> class.</returns>
    public D1VersionClient(GrpcChannel channel, ID1Credentials credentials)
    {
        client = new(channel);
        this.credentials = credentials;
    }

    /// <inheritdoc />
    public async Task<VersionResponse> VersionAsync()
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = await client.VersionAsync(new Protobuf.VersionRequest(), metadata).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /// <inheritdoc />
    public VersionResponse Version()
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = client.Version(new Protobuf.VersionRequest(), metadata);

        return new VersionResponse(response.Commit, response.Tag);
    }
}
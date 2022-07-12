using CyberCrypt.D1.Client.Response;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Version client
/// </summary>
public interface ID1Version
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
public class D1VersionClient : ID1Version
{
    private readonly Protobuf.Version.Version.VersionClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1BaseClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1VersionClient"/> class.</returns>
    public D1VersionClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task<VersionResponse> VersionAsync()
    {
        var response = await client.VersionAsync(new Protobuf.Version.VersionRequest()).ConfigureAwait(false);

        return new VersionResponse(response.Commit, response.Tag);
    }

    /// <inheritdoc />
    public VersionResponse Version()
    {
        var response = client.Version(new Protobuf.Version.VersionRequest());

        return new VersionResponse(response.Commit, response.Tag);
    }
}

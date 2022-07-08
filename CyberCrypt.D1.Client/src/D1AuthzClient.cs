using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.Response;
using CyberCrypt.D1.Client.Utils;
using Grpc.Core;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Authz client
/// </summary>
public interface ID1AuthzClient
{
    /// <summary>
    /// Give a group permission to access an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="groupId">The ID of the group.</param>
    void AddPermission(string objectId, string groupId);

    /// <inheritdoc cref="AddPermission"/>
    Task AddPermissionAsync(string objectId, string groupId);

    /// <summary>
    /// Get the permissions applied to an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <returns>An instance of <see cref="GetPermissionsResponse" />.</returns>
    GetPermissionsResponse GetPermissions(string objectId);

    /// <inheritdoc cref="GetPermissions"/>
    Task<GetPermissionsResponse> GetPermissionsAsync(string objectId);

    /// <summary>
    /// Revoke a groups permission to access an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="groupId">The ID of the group.</param>
    void RemovePermission(string objectId, string groupId);

    /// <inheritdoc cref="RemovePermission"/>
    Task RemovePermissionAsync(string objectId, string groupId);
}

/// <summary>
/// Authz client for connection to a D1 server.
/// </summary>
public class D1AuthzClient : ID1AuthzClient
{
    private readonly Protobuf.Authz.AuthzClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1AuthzClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1AuthzClient"/> class.</returns>
    public D1AuthzClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task<GetPermissionsResponse> GetPermissionsAsync(string objectId)
    {
        var response = await client.GetPermissionsAsync(new Protobuf.GetPermissionsRequest { ObjectId = objectId })
            .ConfigureAwait(false);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public GetPermissionsResponse GetPermissions(string objectId)
    {
        var response = client.GetPermissions(new Protobuf.GetPermissionsRequest { ObjectId = objectId });

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public async Task AddPermissionAsync(string objectId, string groupId)
    {
        await client.AddPermissionAsync(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId })
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddPermission(string objectId, string groupId)
    {
        client.AddPermission(new Protobuf.AddPermissionRequest { ObjectId = objectId, GroupId = groupId });
    }

    /// <inheritdoc />
    public async Task RemovePermissionAsync(string objectId, string groupId)
    {
        await client.RemovePermissionAsync(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId })
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemovePermission(string objectId, string groupId)
    {
        client.RemovePermission(new Protobuf.RemovePermissionRequest { ObjectId = objectId, GroupId = groupId });
    }
}

using CyberCrypt.D1.Client.Response;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Authz client
/// </summary>
public interface ID1Authz
{
    /// <summary>
    /// Give a group permission to access an object.
    /// </summary>
    /// <param name="objectId">The ID of the object.</param>
    /// <param name="groupIds">The ID of the groups.</param>
    void AddPermission(string objectId, IEnumerable<string> groupIds);

    /// <inheritdoc cref="AddPermission"/>
    Task AddPermissionAsync(string objectId, IEnumerable<string> groupIds);

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
    /// <param name="groupIds">The ID of the group.</param>
    void RemovePermission(string objectId, IEnumerable<string> groupIds);

    /// <inheritdoc cref="RemovePermission"/>
    Task RemovePermissionAsync(string objectId, IEnumerable<string> groupIds);
    
    /// <inheritdoc cref="CheckPermission"/>
    Task<bool> CheckPermissionAsync(string objectId);
    
    /// <summary>
    /// Check if the user has access to an object.
    /// </summary>
    /// <param name="objectId">The object ID.</param>
    bool CheckPermission(string objectId);
}

/// <summary>
/// Authz client for connection to a D1 server.
/// </summary>
public class D1AuthzClient : ID1Authz
{
    private readonly Protobuf.Authz.Authz.AuthzClient client;

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
        var response = await client.GetPermissionsAsync(new Protobuf.Authz.GetPermissionsRequest { ObjectId = objectId })
            .ConfigureAwait(false);

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public GetPermissionsResponse GetPermissions(string objectId)
    {
        var response = client.GetPermissions(new Protobuf.Authz.GetPermissionsRequest { ObjectId = objectId });

        return new GetPermissionsResponse(new List<string>(response.GroupIds));
    }

    /// <inheritdoc />
    public async Task AddPermissionAsync(string objectId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authz.AddPermissionRequest { ObjectId = objectId };
        request.GroupIds.AddRange(groupIds);
        await client.AddPermissionAsync(request)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddPermission(string objectId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authz.AddPermissionRequest { ObjectId = objectId };
        request.GroupIds.AddRange(groupIds);
        client.AddPermission(request);
    }

    /// <inheritdoc />
    public async Task RemovePermissionAsync(string objectId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authz.RemovePermissionRequest { ObjectId = objectId };
        request.GroupIds.AddRange(groupIds);
        await client.RemovePermissionAsync(request)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemovePermission(string objectId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authz.RemovePermissionRequest { ObjectId = objectId };
        request.GroupIds.AddRange(groupIds);
        client.RemovePermission(request);
    }

    /// <inheritdoc />
    public async Task<bool> CheckPermissionAsync(string objectId)
    {
        var response = await client.CheckPermissionAsync(new Protobuf.Authz.CheckPermissionRequest { ObjectId = objectId });
        return response.HasPermission;
    }

    /// <inheritdoc />
    public bool CheckPermission(string objectId)
    {
        var response = client.CheckPermission(new Protobuf.Authz.CheckPermissionRequest { ObjectId = objectId });
        return response.HasPermission;
    }
}

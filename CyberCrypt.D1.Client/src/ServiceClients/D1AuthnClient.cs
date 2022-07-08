using CyberCrypt.D1.Client.Response;
using CyberCrypt.D1.Client.Utils;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Authn client
/// </summary>
public interface ID1AuthnClient
{
    /// <summary>
    /// Add a user to a group.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="groupId">The ID of group.</param>
    void AddUserToGroup(string userId, string groupId);

    /// <inheritdoc cref="AddUserToGroup"/>
    Task AddUserToGroupAsync(string userId, string groupId);

    /// <summary>
    /// Create a new group.
    /// </summary>
    /// <param name="scopes">List of scopes assigned to the new group.</param>
    /// <returns>An instance of <see cref="CreateGroupResponse"/>.</returns>
    CreateGroupResponse CreateGroup(IList<Scope> scopes);

    /// <inheritdoc cref="CreateGroup"/>
    Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes);

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="scopes">List of scopes assigned to the new user.</param>
    /// <returns>An instance of <see cref="CreateUserResponse"/>.</returns>
    CreateUserResponse CreateUser(IList<Scope> scopes);

    /// <inheritdoc cref="CreateUser"/>
    Task<CreateUserResponse> CreateUserAsync(IList<Scope> scopes);

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="userId">ID of the user to delete.</param>
    void RemoveUser(string userId);

    /// <inheritdoc cref="RemoveUser"/>
    Task RemoveUserAsync(string userId);

    /// <summary>
    /// Remove a user from a group.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="groupId">The ID of group.</param>
    void RemoveUserFromGroup(string userId, string groupId);

    /// <inheritdoc cref="RemoveUserFromGroup"/>
    Task RemoveUserFromGroupAsync(string userId, string groupId);
}

/// <summary>
/// Authn client for connection to a D1 server.
/// </summary>
public class D1AuthnClient : ID1AuthnClient
{
    private readonly Protobuf.Authn.AuthnClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1AuthnClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1AuthnClient"/> class.</returns>
    public D1AuthnClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task<CreateUserResponse> CreateUserAsync(IList<Scope> scopes)
    {
        var request = new Protobuf.CreateUserRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await client.CreateUserAsync(request).ConfigureAwait(false);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    /// <inheritdoc />
    public CreateUserResponse CreateUser(IList<Scope> scopes)
    {
        var request = new Protobuf.CreateUserRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = client.CreateUser(request);

        return new CreateUserResponse(response.UserId, response.Password);
    }

    /// <inheritdoc />
    public async Task RemoveUserAsync(string userId)
    {
        await client.RemoveUserAsync(new Protobuf.RemoveUserRequest { UserId = userId }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUser(string userId)
    {
        client.RemoveUser(new Protobuf.RemoveUserRequest { UserId = userId });
    }

    /// <inheritdoc />
    public async Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes)
    {
        var request = new Protobuf.CreateGroupRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = await client.CreateGroupAsync(request).ConfigureAwait(false);

        return new CreateGroupResponse(response.GroupId);
    }

    /// <inheritdoc />
    public CreateGroupResponse CreateGroup(IList<Scope> scopes)
    {
        var request = new Protobuf.CreateGroupRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = client.CreateGroup(request);

        return new CreateGroupResponse(response.GroupId);
    }

    /// <inheritdoc />
    public async Task AddUserToGroupAsync(string userId, string groupId)
    {
        await client.AddUserToGroupAsync(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId })
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddUserToGroup(string userId, string groupId)
    {
        client.AddUserToGroup(new Protobuf.AddUserToGroupRequest { UserId = userId, GroupId = groupId });
    }

    /// <inheritdoc />
    public async Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        await client.RemoveUserFromGroupAsync(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUserFromGroup(string userId, string groupId)
    {
        client.RemoveUserFromGroup(new Protobuf.RemoveUserFromGroupRequest { UserId = userId, GroupId = groupId });
    }
}
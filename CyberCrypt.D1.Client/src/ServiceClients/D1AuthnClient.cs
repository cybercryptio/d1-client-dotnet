// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CyberCrypt.D1.Client.Response;
using CyberCrypt.D1.Client.Utils;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Authn client
/// </summary>
public interface ID1Authn
{
    /// <summary>
    /// Add a user to a group.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="groupIds">The ID of groups.</param>
    void AddUserToGroups(string userId, IEnumerable<string> groupIds);

    /// <inheritdoc cref="AddUserToGroups"/>
    Task AddUserToGroupsAsync(string userId, IEnumerable<string> groupIds);

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
    /// <param name="groupIds">The ID of groups.</param>
    void RemoveUserFromGroups(string userId, IEnumerable<string> groupIds);

    /// <inheritdoc cref="RemoveUserFromGroups"/>
    Task RemoveUserFromGroupsAsync(string userId, IEnumerable<string> groupIds);
}

/// <summary>
/// Authn client for connection to a D1 server.
/// </summary>
public class D1AuthnClient : ID1Authn
{
    private readonly Protobuf.Authn.Authn.AuthnClient client;

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
        var request = new Protobuf.Authn.CreateUserRequest();
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
        var request = new Protobuf.Authn.CreateUserRequest();
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
        await client.RemoveUserAsync(new Protobuf.Authn.RemoveUserRequest { UserId = userId }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUser(string userId)
    {
        client.RemoveUser(new Protobuf.Authn.RemoveUserRequest { UserId = userId });
    }

    /// <inheritdoc />
    public async Task<CreateGroupResponse> CreateGroupAsync(IList<Scope> scopes)
    {
        var request = new Protobuf.Authn.CreateGroupRequest();
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
        var request = new Protobuf.Authn.CreateGroupRequest();
        foreach (Scope scope in scopes)
        {
            request.Scopes.Add(scope.GetServiceScope());
        }

        var response = client.CreateGroup(request);

        return new CreateGroupResponse(response.GroupId);
    }

    /// <inheritdoc />
    public async Task AddUserToGroupsAsync(string userId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authn.AddUserToGroupsRequest { UserId = userId };
        request.GroupIds.AddRange(groupIds);
        await client.AddUserToGroupsAsync(request)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddUserToGroups(string userId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authn.AddUserToGroupsRequest { UserId = userId };
        request.GroupIds.AddRange(groupIds);
        client.AddUserToGroups(request);
    }

    /// <inheritdoc />
    public async Task RemoveUserFromGroupsAsync(string userId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authn.RemoveUserFromGroupsRequest { UserId = userId };
        request.GroupIds.AddRange(groupIds);
        await client.RemoveUserFromGroupsAsync(request).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void RemoveUserFromGroups(string userId, IEnumerable<string> groupIds)
    {
        var request = new Protobuf.Authn.RemoveUserFromGroupsRequest { UserId = userId };
        request.GroupIds.AddRange(groupIds);
        client.RemoveUserFromGroups(request);
    }
}

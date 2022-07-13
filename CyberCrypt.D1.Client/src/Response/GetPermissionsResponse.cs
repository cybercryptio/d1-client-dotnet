// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ServiceClients.ID1Authz.GetPermissions"/> or <see cref="ServiceClients.ID1Authz.GetPermissionsAsync"/>.
/// </summary>
public class GetPermissionsResponse {
    /// <summary>
    /// Gets the group ids.
    /// </summary>
    public List<string> GroupIds { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPermissionsResponse"/>.
    /// </summary>
    /// <param name="groupIds">The group ids.</param>
    public GetPermissionsResponse(List<string> groupIds) {
        GroupIds = groupIds;
    }
}

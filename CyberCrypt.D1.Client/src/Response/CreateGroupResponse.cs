// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ServiceClients.ID1Authn.CreateGroup"/> or <see cref="ServiceClients.ID1Authn.CreateGroupAsync"/>.
/// </summary>
public class CreateGroupResponse {
    /// <summary>
    /// Gets the group id.
    /// </summary>
    public string GroupId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateGroupResponse"/>.
    /// </summary>
    /// <param name="groupId">The group id.</param>
    public CreateGroupResponse(string groupId) {
        GroupId = groupId;
    }
}

// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ID1Base.CreateGroup"/> or <see cref="ID1Base.CreateGroupAsync"/>.
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

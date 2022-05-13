// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

/// <summary>
/// Response from <see cref="IEncryptonizeBase.CreateGroup"/> or <see cref="IEncryptonizeBase.CreateGroupAsync"/>.
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

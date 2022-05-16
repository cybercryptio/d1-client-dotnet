// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

/// <summary>
/// Response from <see cref="IEncryptonizeBase.GetPermissions"/> or <see cref="IEncryptonizeBase.GetPermissionsAsync"/>.
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

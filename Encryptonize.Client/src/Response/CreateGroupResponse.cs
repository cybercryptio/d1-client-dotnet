// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class CreateGroupResponse {
    private string groupId;
    public string GroupId { get { return groupId; } }

    public CreateGroupResponse(string groupId) {
        this.groupId = groupId;
    }
}

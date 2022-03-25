// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class CreateGroupResponse {
    public string GroupId { get; private set; }

    public CreateGroupResponse(string GroupId) {
        this.GroupId = GroupId;
    }
}

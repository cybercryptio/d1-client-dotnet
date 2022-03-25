// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class GetPermissionsResponse {
    public List<string> GroupIds { get; private set; }

    public GetPermissionsResponse(List<string> GroupIds) {
        this.GroupIds = GroupIds;
    }
}

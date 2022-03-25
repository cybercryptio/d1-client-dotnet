// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class GetPermissionsResponse {
    private List<string> groupIds;
    public List<string> GroupIds { get { return groupIds; } }

    public GetPermissionsResponse(List<string> groupIds) {
        this.groupIds = groupIds;
    }
}

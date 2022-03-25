// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class VersionResponse {
    private string commit;
    public string Commit { get { return commit; } }
    private string tag;
    public string Tag { get { return tag; } }

    public VersionResponse(string commit, string tag) {
        this.commit = commit;
        this.tag = tag;
    }
}

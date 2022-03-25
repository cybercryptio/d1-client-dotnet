// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

public class VersionResponse {
    public string Commit { get; private set; }
    public string Tag { get; private set; }

    public VersionResponse(string Commit, string Tag) {
        this.Commit = Commit;
        this.Tag = Tag;
    }
}

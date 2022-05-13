// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

/// <summary>
/// Response from <see cref="IEncryptonizeBase.Version"/> or <see cref="IEncryptonizeBase.VersionAsync"/>.
/// </summary>
public class VersionResponse {
    /// <summary>
    /// Gets the Git commit.
    /// </summary>
    public string Commit { get; private set; }

    /// <summary>
    /// Gets the Git tag.
    /// </summary>
    public string Tag { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionResponse"/>.
    /// </summary>
    /// <param name="commit">The Git commit.</param>
    /// <param name="tag">The Git tag.</param>
    public VersionResponse(string commit, string tag) {
        Commit = commit;
        Tag = tag;
    }
}

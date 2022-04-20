// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Response;

/// <summary>
/// Response from <see cref="IEncryptonizeClient.Retrieve" /> or <see cref="IEncryptonizeClient.RetrieveAsync" />.
/// </summary>
public class RetrieveResponse {
    /// <summary>
    /// Gets the plaintext.
    /// </summary>
    public byte[] Plaintext { get; private set; }

    /// <summary>
    /// Gets the associated data.
    /// </summary>
    public byte[] AssociatedData { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetrieveResponse"/>.
    /// </summary>
    public RetrieveResponse(byte[] plaintext, byte[] associatedData) {
        Plaintext = plaintext;
        AssociatedData = associatedData;
    }
}

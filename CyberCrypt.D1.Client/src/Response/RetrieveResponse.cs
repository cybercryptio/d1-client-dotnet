// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ID1StoreClient.Retrieve" /> or <see cref="ID1StoreClient.RetrieveAsync" />.
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
    /// <param name="plaintext">The plaintext.</param>
    /// <param name="associatedData">The associated data.</param>
    public RetrieveResponse(byte[] plaintext, byte[] associatedData) {
        Plaintext = plaintext;
        AssociatedData = associatedData;
    }
}

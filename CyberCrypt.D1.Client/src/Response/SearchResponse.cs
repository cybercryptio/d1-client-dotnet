// Copyright 2020-2022 CYBERCRYPT
namespace CyberCrypt.D1.Client.Response;

/// <summary>
/// Response from <see cref="ID1SearchableClient.Search"/> or <see cref="ID1SearchableClient.SearchAsync"/>.
/// </summary>
public class SearchResponse
{
    /// <summary>
    /// Gets the identifiers.
    /// </summary>
    public List<string> Identifiers { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResponse"/>.
    /// </summary>
    /// <param name="identifiers">The identifiers.</param>
    public SearchResponse(List<string> identifiers)
    {
        Identifiers = identifiers;
    }
}

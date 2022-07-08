using CyberCrypt.D1.Client.Credentials;
using CyberCrypt.D1.Client.Response;
using CyberCrypt.D1.Client.Utils;
using Grpc.Core;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Interface for Searchable client
/// </summary>
public interface ID1Searchable
{
    /// <summary>
    /// Add keywords/identifier pairs to secure index.
    /// </summary>
    /// <param name="keywords">The keywords.</param>
    /// <param name="identifier">The identifier.</param>
    void Add(List<string> keywords, string identifier);

    /// <inheritdoc cref="Add"/>
    Task AddAsync(List<string> keywords, string identifier);

    /// <summary>
    /// Search for keyword in secure index..
    /// </summary>
    /// <param name="keyword">The keyword to search for.</param>
    /// <returns>An instance of <see cref="SearchResponse"/>.</returns>
    SearchResponse Search(string keyword);

    /// <inheritdoc cref="Search"/>
    Task<SearchResponse> SearchAsync(string keyword);

    /// <summary>
    /// Delete keywords/identifier pairs from secure index.
    /// </summary>
    /// <param name="keywords">The keywords.</param>
    /// <param name="identifier">The identifier.</param>
    void Delete(List<string> keywords, string identifier);

    /// <inheritdoc cref="Delete"/>
    Task DeleteAsync(List<string> keywords, string identifier);
}

/// <summary>
/// Searchable client for connection to a D1 server.
/// </summary>
public class D1SearchableClient : ID1Searchable
{
    private readonly Protobuf.Searchable.Searchable.SearchableClient client;
    private readonly ID1Credentials credentials;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1SearchableClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <param name="credentials">Credentials used to authenticate with D1.</param>
    /// <returns>A new instance of the <see cref="D1SearchableClient"/> class.</returns>
    public D1SearchableClient(GrpcChannel channel, ID1Credentials credentials)
    {
        client = new(channel);
        this.credentials = credentials;
    }

    /// <inheritdoc />
    public async Task AddAsync(List<string> keywords, string identifier)
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");

        var response = new Protobuf.Searchable.AddRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        await client.AddAsync(response, metadata).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Add(List<string> keywords, string identifier)
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");

        var response = new Protobuf.Searchable.AddRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        client.Add(response, metadata);
    }

    /// <inheritdoc />
    public async Task<SearchResponse> SearchAsync(string keyword)
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = await client.SearchAsync(new Protobuf.Searchable.SearchRequest
        {
            Keyword = keyword
        }, metadata).ConfigureAwait(false);

        return new SearchResponse(new List<string>(response.Identifiers));
    }

    /// <inheritdoc />
    public SearchResponse Search(string keyword)
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");
        var response = client.Search(new Protobuf.Searchable.SearchRequest
        {
            Keyword = keyword
        }, metadata);

        return new SearchResponse(new List<string>(response.Identifiers));
    }

    /// <inheritdoc />
    public async Task DeleteAsync(List<string> keywords, string identifier)
    {
        var token = await credentials.GetTokenAsync();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");

        var response = new Protobuf.Searchable.DeleteRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        await client.DeleteAsync(response, metadata).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Delete(List<string> keywords, string identifier)
    {
        var token = credentials.GetToken();
        var metadata = new Metadata();
        metadata.Add("Authorization", $"Bearer {token}");

        var response = new Protobuf.Searchable.DeleteRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        client.Delete(response, metadata);
    }
}
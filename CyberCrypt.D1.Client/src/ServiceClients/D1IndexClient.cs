using CyberCrypt.D1.Client.Response;
using Grpc.Net.Client;

namespace CyberCrypt.D1.Client.ServiceClients;

/// <summary>
/// Interface for Index client
/// </summary>
public interface ID1Index
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
/// Index client for connection to a D1 server.
/// </summary>
public class D1IndexClient : ID1Index
{
    private readonly Protobuf.Index.Index.IndexClient client;

    /// <summary>
    /// Initialize a new instance of the <see cref="D1IndexClient"/> class.
    /// </summary>
    /// <param name="channel">gRPC channel.</param>
    /// <returns>A new instance of the <see cref="D1IndexClient"/> class.</returns>
    public D1IndexClient(GrpcChannel channel)
    {
        client = new(channel);
    }

    /// <inheritdoc />
    public async Task AddAsync(List<string> keywords, string identifier)
    {
        var response = new Protobuf.Index.AddRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        await client.AddAsync(response).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Add(List<string> keywords, string identifier)
    {
        var response = new Protobuf.Index.AddRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        client.Add(response);
    }

    /// <inheritdoc />
    public async Task<SearchResponse> SearchAsync(string keyword)
    {
        var response = await client.SearchAsync(new Protobuf.Index.SearchRequest
        {
            Keyword = keyword
        }).ConfigureAwait(false);

        return new SearchResponse(new List<string>(response.Identifiers));
    }

    /// <inheritdoc />
    public SearchResponse Search(string keyword)
    {
        var response = client.Search(new Protobuf.Index.SearchRequest
        {
            Keyword = keyword
        });

        return new SearchResponse(new List<string>(response.Identifiers));
    }

    /// <inheritdoc />
    public async Task DeleteAsync(List<string> keywords, string identifier)
    {
        var response = new Protobuf.Index.DeleteRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        await client.DeleteAsync(response).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Delete(List<string> keywords, string identifier)
    {
        var response = new Protobuf.Index.DeleteRequest { Identifier = identifier };
        response.Keywords.AddRange(keywords);
        client.Delete(response);
    }
}
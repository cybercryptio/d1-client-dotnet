// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
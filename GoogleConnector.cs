using SkTestApp.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace SkTestApp.Handlers.Connectors
{

    /// <summary>
    /// Google API connector.
    /// </summary>
    public class GoogleConnector : IWebSearchEngineConnector, IDisposable
    {
        private readonly IOptions<WebSearchOptions> _options;
        private readonly ILogger _logger;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;

        public GoogleConnector(IOptions<WebSearchOptions> options, ILogger<GoogleConnector>? logger = null)
        {
            _options = options;
            this._logger = logger ?? NullLogger<GoogleConnector>.Instance;
            this._httpClientHandler = new() { CheckCertificateRevocationList = true };
            this._httpClient = new HttpClient(this._httpClientHandler);
        }






        /// <inheritdoc/>
        public async Task<string> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            var result = string.Empty;

            string apiKey = _options.Value.GoogleSearchApiKey;
            string cseId = _options.Value.GoogleSearchCseId;
            int resultsCount = 3;

            Uri uri = new($"https://customsearch.googleapis.com/customsearch/v1?key={apiKey}&cx={cseId}&num={resultsCount}&q={Uri.EscapeDataString(query)}");


            this._logger.LogDebug("WebSearch Sending request: {0}", uri);
            HttpResponseMessage response = await this._httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            this._logger.LogDebug("WebSearch Response received: {0}", response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();
            this._logger.LogTrace("WebSearch Response content received: {0}", json);

            GoogleSearchResponse? data = JsonSerializer.Deserialize<GoogleSearchResponse>(json);
            Item? firstResult = data?.Items?.FirstOrDefault();

            if (firstResult == null)
            {
                _logger.LogWarning("WebSearch - No results found for query:");
            }
            else result = ParseSnippetsAndUrlsAsString(data) ?? string.Empty;

            this._logger.LogDebug("Google Result: {0}, {1}, {2}",
                firstResult?.Title,
                firstResult?.Url,
                firstResult?.Snippet);

            return result;
        }

        private string ParseSnippetsAndUrlsAsString(GoogleSearchResponse gooogleResponse)
        {
            var result = "";
            foreach (var item in gooogleResponse.Items)
            {
                string title = item.Title;
                string snippet = item.Snippet;
                string url = Uri.EscapeDataString(item.Url);
                result += $"{title}\n{url}\n{snippet}\n\n";
            }
            return result;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._httpClient.Dispose();
                this._httpClientHandler.Dispose();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        [SuppressMessage("Performance", "CA1812:Internal class that is apparently never instantiated",
            Justification = "Class is instantiated through deserialization.")]
        private sealed class GoogleSearchResponse
        {
            [JsonPropertyName("items")]
            public Item[]? Items { get; set; }
        }

        [SuppressMessage("Performance", "CA1812:Internal class that is apparently never instantiated",
            Justification = "Class is instantiated through deserialization.")]
        private sealed class Item
        {
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("formattedUrl")]
            public string Url { get; set; } = string.Empty;

            [JsonPropertyName("snippet")]
            public string Snippet { get; set; } = string.Empty;
        }
    }



}

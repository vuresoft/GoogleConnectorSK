using SkTestApp.Configuration;
using SkTestApp.Handlers.Connectors;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SkTestApp.Handlers.Skills.WebSearch
{
    /// <summary>
    /// Web search engine skill (e.g. Bing)
    /// </summary>
    public class WebSearchEngineSkill
    {
        private readonly IWebSearchEngineConnector _connector;

        public WebSearchEngineSkill(IWebSearchEngineConnector connector)
        {
            _connector = connector;
        }

        [SKFunction("Perform a web search")]
        [SKFunctionInput(Description = "Text to search for")]
        [SKFunctionName("Search")]
        public async Task<string> SearchAsync(string query, SKContext context)
        {

            string result = await this._connector.SearchAsync(query, context.CancellationToken);
            if (string.IsNullOrWhiteSpace(result))
            {
                context.Fail("Failed to get a response from the web search engine.");
            }

            return result;
        }
    }
}

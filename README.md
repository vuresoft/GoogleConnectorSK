# Google Search API connector for MS Semantic Kernel

- Simple connector forked from the existing Bing connector.
- Setup a Google search connector at https://developers.google.com/custom-search/v1/overview
- Add config values (apikey and custom google search engine ID) to settings file
- Add connector through dependancy injection :

```c#
      //services.AddScoped<IWebSearchEngineConnector, BingConnector>();
       services.AddScoped<IWebSearchEngineConnector, GoogleConnector>();
```
- Use through the existing WebSearchSkill in SK

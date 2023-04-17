# Google Search API connector for MS Semantic Kernel

- Simple connector forked from the existing Bing connector.
- Setup a Google search connector at https://developers.google.com/custom-search/v1/overview
- Add config values (apikey and custom google search engine ID) to settings file
      - see example: https://github.com/vuresoft/GoogleConnectorSK/blob/main/settings.json
- Add config properties to the WebSearchOptions file (for strongly typed config)
      - see example: https://github.com/vuresoft/GoogleConnectorSK/blob/main/WebSearchOptions.cs     
- Add connector through dependancy injection :

```c#
      //services.AddScoped<IWebSearchEngineConnector, BingConnector>();
       services.AddScoped<IWebSearchEngineConnector, GoogleConnector>();
```

- Use it through the WebSearchSkill via Dependancy Injection
```
        services.AddScoped<WebSearchEngineSkill>();
        
```


## NOTES:
1. When you first setup google custom search, the first API query might return errors until you validate with the link provided. Try it in a browser first to get the json response, then click the provided link to enable the API.
2. Response from search is slighlty modified (from bing version) to return Title, URL and Snippet.

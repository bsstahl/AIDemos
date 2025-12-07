using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using Xunit.Abstractions;

namespace BearyMC.Test;

public class MCPClient_CallToolAsync_Should
{
    const string _mcpRelativePath = @"..\..\..\..\BearyMC\BearyMC.csproj";

    private readonly ITestOutputHelper _output;
    private readonly IMcpClient _mcpClient;

    public MCPClient_CallToolAsync_Should(ITestOutputHelper output)
    {
        _output = output;

        var services = new ServiceCollection()
            // TODO: Add the XUnit test logger to the service collection
            .BuildServiceProvider();

        // Create the MCP client
        var mcpClientTask = McpClientFactory.CreateAsync(
            new StdioClientTransport(new()
            {
                Command = "dotnet run",
                Arguments = ["--project", Path.GetFullPath(_mcpRelativePath)],
                Name = "Beary",
            }));
        Task.WaitAll(mcpClientTask);
        _mcpClient = mcpClientTask.Result;
    }

    [Fact]
    public async Task ReturnASuccessResponse()
    {
        // _output.WriteLine($"Connected to MCP server '{_mcpClient.ServerInfo.Name}' with PID {_mcpClient}");
        var serverInfo = System.Text.Json.JsonSerializer.Serialize(_mcpClient, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        //_output.WriteLine(serverInfo);

        var toolName = "GetArticleList";
        var arguments = new Dictionary<string, object?>
        {
            { "logger",  new XUnitLogger(_output)},
            { "searchClient", new Beary.Documents.Search(null, null, null) },
            { "searchQuery", "Best ways to learn about my problem domain" } // Example search query for the ArticlesListTool
        };

        var response = await _mcpClient.CallToolAsync(toolName, arguments);
        var responseText = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        _output.WriteLine(responseText);

        //_output.WriteLine("Available prompts:");
        //foreach (var prompt in response.Content)
        //    _output.WriteLine($"{prompt}");

        //Assert.NotEmpty(prompts);
    }
}

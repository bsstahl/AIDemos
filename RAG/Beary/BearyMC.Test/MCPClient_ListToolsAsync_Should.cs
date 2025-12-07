using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Xunit.Abstractions;

namespace BearyMC.Test;

public class MCPClient_ListToolsAsync_Should
{
    const string _mcpRelativePath = @"..\..\..\..\BearyMC\BearyMC.csproj";

    private readonly ITestOutputHelper _output;
    private readonly IMcpClient _mcpClient;

    public MCPClient_ListToolsAsync_Should(ITestOutputHelper output)
    {
        _output = output;

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
    public async Task ReturnAListOfAvailableTools()
    {
        _output.WriteLine($"Connected to MCP server: {_mcpClient.ServerInfo.Name}");

        // List all available tools from the MCP server.
        _output.WriteLine("Available tools:");
        var tools = await _mcpClient.ListToolsAsync();
        foreach (var tool in tools)
            _output.WriteLine($"{tool}");

        Assert.NotEmpty(tools);
    }
}

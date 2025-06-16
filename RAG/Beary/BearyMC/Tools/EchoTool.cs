using System;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace BearyMC.Tools;

// Define a static class to hold MCP tools.
[McpServerToolType]
public static class EchoTool
{
    // Expose a tool that echoes the input message back to the client.
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    // Expose a tool that returns the input message in reverse.
    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());
}
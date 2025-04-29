using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

// Create a host builder for our MCP server
var builder = Host.CreateApplicationBuilder(args);

// Configure logging to send all logs to stderr for better cross-platform compatibility
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Configure our MCP server with standard stdio server transport
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(); // Auto-register all tools in this assembly

// Build and run the host
await builder.Build().RunAsync();

/// <summary>
/// Unity-related tools for MCP integration
/// </summary>
[McpServerToolType]
public static class UnityTools
{
    [McpServerTool, Description("Echo a message back to the client")]
    public static string Echo(string message)
    {
        return $"Unity MCP Server says: {message}";
    }

    [McpServerTool, Description("Get information about the MCP server")]
    public static string GetServerInfo()
    {
        return "Unity MCP Server v0.1.0";
    }

    [McpServerTool, Description("A sample tool for Unity to work with JSON data")]
    public static string ProcessJsonData(string jsonData)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
            return $"Successfully processed JSON data with {data.EnumerateObject().Count()} properties";
        }
        catch (Exception ex)
        {
            return $"Error processing JSON data: {ex.Message}";
        }
    }

    [McpServerTool, Description("A sample tool to test Unity GameObject operations")]
    public static string SimulateGameObjectOperation(string objectName, string operation)
    {
        return $"Simulated {operation} on GameObject '{objectName}' successfully";
    }
}
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

Console.WriteLine("Starting Unity MCP Server...");

// Configure our MCP server with standard stdio server transport
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(); // Auto-register all tools in this assembly

// Build and run the host
var host = builder.Build();
Console.WriteLine("Server built, running...");
Console.WriteLine("Unity MCP Server is ready for connections.");

await host.RunAsync();

/// <summary>
/// Unity-related tools for MCP integration
/// </summary>
[McpServerToolType]
public static class UnityTools
{
    private static int _totalRequests = 0;
    private static Dictionary<string, int> _toolUsage = new Dictionary<string, int>();

    private static void RecordToolCall(string toolName)
    {
        _totalRequests++;
        if (_toolUsage.ContainsKey(toolName))
        {
            _toolUsage[toolName]++;
        }
        else
        {
            _toolUsage[toolName] = 1;
        }
        
        Console.WriteLine($"Tool call: {toolName} (Total requests: {_totalRequests})");
    }

    [McpServerTool, Description("Echo a message back to the client")]
    public static string Echo(string message)
    {
        RecordToolCall("Echo");
        Console.WriteLine($"Echo tool called with message: {message}");
        return $"Unity MCP Server says: {message}";
    }

    [McpServerTool, Description("Get information about the MCP server")]
    public static string GetServerInfo()
    {
        RecordToolCall("GetServerInfo");
        var info = new
        {
            Version = "0.1.0",
            Tools = _toolUsage.Count,
            TotalRequests = _totalRequests,
            Runtime = Environment.Version.ToString()
        };
        
        return JsonSerializer.Serialize(info);
    }

    [McpServerTool, Description("A sample tool for Unity to work with JSON data")]
    public static string ProcessJsonData(string jsonData)
    {
        RecordToolCall("ProcessJsonData");
        Console.WriteLine($"ProcessJsonData tool called with data: {jsonData}");
        
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
            return $"Successfully processed JSON data with {data.EnumerateObject().Count()} properties";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ProcessJsonData: {ex.Message}");
            return $"Error processing JSON data: {ex.Message}";
        }
    }

    [McpServerTool, Description("A sample tool to test Unity GameObject operations")]
    public static string SimulateGameObjectOperation(string objectName, string operation)
    {
        RecordToolCall("SimulateGameObjectOperation");
        Console.WriteLine($"SimulateGameObjectOperation tool called: {operation} on {objectName}");
        
        // Simulate different operations
        switch (operation.ToLower())
        {
            case "move":
                return $"Moved GameObject '{objectName}' to position (1, 2, 3)";
                
            case "rotate":
                return $"Rotated GameObject '{objectName}' to euler angles (45, 90, 0)";
                
            case "scale":
                return $"Scaled GameObject '{objectName}' to (2, 2, 2)";
                
            case "destroy":
                return $"Destroyed GameObject '{objectName}'";
                
            default:
                return $"Simulated {operation} on GameObject '{objectName}' successfully";
        }
    }
    
    [McpServerTool, Description("Get a list of all available tools")]
    public static string ListTools()
    {
        RecordToolCall("ListTools");
        
        var toolInfos = new[]
        {
            new { Name = "Echo", Description = "Echo a message back to the client" },
            new { Name = "GetServerInfo", Description = "Get information about the MCP server" },
            new { Name = "ProcessJsonData", Description = "A sample tool for Unity to work with JSON data" },
            new { Name = "SimulateGameObjectOperation", Description = "A sample tool to test Unity GameObject operations" },
            new { Name = "ListTools", Description = "Get a list of all available tools" }
        };
        
        return JsonSerializer.Serialize(toolInfos);
    }
}
# Unity MCP Server

A lightweight Model Context Protocol (MCP) server designed to work with Unity applications. This server provides a standardized way for Unity applications to interact with Large Language Models (LLMs).

## Features

- Cross-platform compatibility (Windows, macOS, Linux)
- Simple stdio-based transport for easy integration
- Sample Unity-specific tool implementations
- Built on the official [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)

## Getting Started

### Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download) or later
- Unity 2021.3 or later (for Unity client integration)

### Installation

1. Clone this repository:
   ```
   git clone https://github.com/UnitySnippets/unity-mcp-server.git
   ```

2. Build the server:
   ```
   cd unity-mcp-server/UnityMcpServer
   dotnet build
   ```

3. Run the server:
   ```
   dotnet run
   ```

## Usage with Unity

To use this MCP server with Unity, you'll need to:

1. Create a new Unity project or use an existing one
2. Add the MCP C# SDK to your Unity project (via NuGet or direct reference)
3. Implement a client that connects to this server

### Example Unity Client Code

```csharp
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Threading.Tasks;
using UnityEngine;

public class McpClientExample : MonoBehaviour
{
    private IMcpClient _client;
    
    async void Start()
    {
        // Path to the MCP server executable
        string serverPath = "/path/to/UnityMcpServer.dll"; 
        
        // Create a transport to the server
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "UnityMcpServer",
            Command = "dotnet",
            Arguments = [serverPath]
        });
        
        // Create the client
        _client = await McpClientFactory.CreateAsync(clientTransport);
        
        // List available tools
        var tools = await _client.ListToolsAsync();
        foreach (var tool in tools)
        {
            Debug.Log($"Tool: {tool.Name} - {tool.Description}");
        }
        
        // Test the Echo tool
        var result = await _client.CallToolAsync(
            "Echo",
            new System.Collections.Generic.Dictionary<string, object?>() 
            { 
                ["message"] = "Hello from Unity!" 
            });
            
        Debug.Log(result.Content.First(c => c.Type == "text").Text);
    }
    
    async void OnDestroy()
    {
        if (_client != null)
        {
            await _client.DisposeAsync();
        }
    }
}
```

## Available Tools

This server implements several sample tools:

- `Echo`: Echoes a message back to the client
- `GetServerInfo`: Returns information about the server
- `ProcessJsonData`: Processes JSON data and returns information about it
- `SimulateGameObjectOperation`: Simulates an operation on a Unity GameObject

## Extending the Server

You can easily add your own tools by:

1. Creating a new class with the `[McpServerToolType]` attribute
2. Adding methods with the `[McpServerTool]` attribute
3. Rebuilding and running the server

Example:

```csharp
[McpServerToolType]
public static class MyCustomTools
{
    [McpServerTool, Description("My custom tool")]
    public static string MyTool(string input)
    {
        return $"Processed: {input}";
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- [Model Context Protocol](https://github.com/modelcontextprotocol)
- [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
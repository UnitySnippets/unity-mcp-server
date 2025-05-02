# Unity MCP Server

A lightweight Model Context Protocol (MCP) server designed to work with Unity applications. This server provides a standardized way for Unity applications to interact with Large Language Models (LLMs).

## Features

- Cross-platform compatibility (Windows, macOS, Linux)
- Simple stdio-based transport for easy integration
- Sample Unity-specific tool implementations
- Built on the official [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
- Enhanced logging for better debugging

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later
- Unity 2022.3.19f1 or later (for Unity client integration)

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

## Proof of Concept Integration

For a complete demo showing integration with Unity, see our [project-unity](https://github.com/UnitySnippets/project-unity) repository. This demonstrates a full proof-of-concept with:

- Simple Unity client implementation
- UI for displaying server responses
- Example tool calls
- No dependencies beyond standard .NET

## Available Tools

This server implements several sample tools:

- `Echo`: Echoes a message back to the client
- `GetServerInfo`: Returns information about the server
- `ListTools`: Lists all available tools
- `ProcessJsonData`: Processes JSON data and returns information about it
- `SimulateGameObjectOperation`: Simulates an operation on a Unity GameObject (move, rotate, scale, destroy)

## Usage with Unity

To use this MCP server with Unity, you'll need to:

1. Build the MCP server with `dotnet build`
2. Reference the built server DLL from your Unity project
3. Implement a client that launches the server as a subprocess
4. Send tool requests via standard input/output streams

### Simple Client Implementation

The simplest way to connect to the server from Unity is to launch it as a subprocess and communicate via stdin/stdout:

```csharp
// Start the MCP server as a subprocess
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "/path/to/UnityMcpServer.dll",
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    }
};

process.Start();

// Send a request to call the Echo tool
string jsonRequest = "{\"type\":\"tool_call\",\"tool\":\"Echo\",\"params\":{\"message\":\"Hello from Unity!\"}}";
await process.StandardInput.WriteLineAsync(jsonRequest);
await process.StandardInput.FlushAsync();

// Read the response
string response = await process.StandardOutput.ReadLineAsync();
Debug.Log($"Response: {response}");
```

### Full MCP Client

For a more robust integration, you can use the official MCP C# SDK:

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
        Console.WriteLine($"MyTool called with: {input}");
        return $"Processed: {input}";
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- [Model Context Protocol](https://github.com/modelcontextprotocol)
- [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
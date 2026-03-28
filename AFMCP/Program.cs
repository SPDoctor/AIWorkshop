using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System;
using System.ComponentModel;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

// dotnet add package Azure.AI.OpenAI --prerelease
// dotnet add package Azure.Identity
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
// dotnet add package ModelContextProtocol --prerelease

var endpoint = Environment.GetEnvironmentVariable("endpoint", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("Azure endpoint is not set.");
var deploymentName = Environment.GetEnvironmentVariable("deploymentname", EnvironmentVariableTarget.User) ?? "gpt4";
var apiKey = Environment.GetEnvironmentVariable("apiKey", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("Azure key is not set.");
var credential = new AzureKeyCredential(apiKey);

// Create the chat client with function tools.
var client = new AzureOpenAIClient(new Uri(endpoint), credential);
Console.WriteLine("current directory: " + Environment.CurrentDirectory);
// Connect to a remote MCP server
var transport = new StdioClientTransport(new()
{
    Command = "dotnet run",
    Arguments = ["--project", "..\\MCPServer"],
    Name = "Minimal MCP Server",
});
var mcpClient = await McpClient.CreateAsync(transport);
// Get available tools from MCP server
var mcpFunctions = await mcpClient.ListToolsAsync();

var agent = client.GetChatClient(deploymentName)
  .CreateAIAgent(
    instructions: "You are a helpful assistant who responds to the user in the style of James Joyce.",
    tools: [..mcpFunctions]
  );

AgentThread thread = agent.GetNewThread();
Console.Write("agent listening > ");
var prompt = Console.ReadLine();
while(prompt != "quit")
{
    var response = await agent.RunAsync(prompt, thread);
    Console.WriteLine(response.Text);
    Console.Write("agent listening > ");
    prompt = Console.ReadLine();
}























// Streaming agent interaction with function tools.
// await foreach (var update in weatherAgent.CompleteStreamingAsync("What is the weather like in Amsterdam?"), chatOptions)
// {
//     Console.WriteLine(update);
// }

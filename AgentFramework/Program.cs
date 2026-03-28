using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System;
using System.ComponentModel;
using Plugins;

var endpoint = Environment.GetEnvironmentVariable("endpoint", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("Azure endpoint is not set.");
var deploymentName = Environment.GetEnvironmentVariable("deploymentname", EnvironmentVariableTarget.User) ?? "gpt4";
var apiKey = Environment.GetEnvironmentVariable("apiKey", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("Azure key is not set.");
var credential = new AzureKeyCredential(apiKey);

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

// Create the chat client with function tools.
var client = new AzureOpenAIClient(new Uri(endpoint), credential);

var weatherAgent = client.GetChatClient(deploymentName)
  .CreateAIAgent(
    instructions: "You are a helpful agent giving weather information.",
    tools: [AIFunctionFactory.Create(GetWeather)]
  );

var agent = client.GetChatClient(deploymentName)
  .CreateAIAgent(
    instructions: "You are a helpful assistant who responds to the user.",
    tools: [
      weatherAgent.AsAIFunction(),
      AIFunctionFactory.Create(LightPlugin.LightOn),
      AIFunctionFactory.Create(LightPlugin.LightOff),
      AIFunctionFactory.Create(LightPlugin.LightFlash),
      AIFunctionFactory.Create(CalcPlugin.Root)
    ]
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

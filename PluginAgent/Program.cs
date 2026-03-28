using Azure;
using Azure.AI.OpenAI; 
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using Plugins;

const string deploymentName = "gpt-5.4";
const string endpoint = "https://billa-workshop-resource.services.ai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("apiKey_gpt-5.4", EnvironmentVariableTarget.User) ?? "";
var credential = new AzureKeyCredential(apiKey);

var client = new AzureOpenAIClient(new Uri(endpoint), credential);
var chatClient = client.GetChatClient(deploymentName);
var agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant who replies in the style of William Shakespeare.",
    tools: [AIFunctionFactory.Create(TimePlugin.Time)]
    );

Console.WriteLine("It beginneth...");
var prompt = Console.ReadLine();
var session = await agent.CreateSessionAsync();
while (prompt != "")
{
    Console.WriteLine(await agent.RunAsync(prompt, session));
    prompt = Console.ReadLine();
}
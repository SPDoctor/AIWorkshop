using System.ComponentModel;
using Azure;
using Azure.AI.OpenAI; 
using Microsoft.Extensions.AI;
using OpenAI.Chat;


const string deploymentName = "gpt-5.4";
const string endpoint = "https://billa-workshop-resource.services.ai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("apiKey_gpt-5.4", EnvironmentVariableTarget.User) ?? "";
var credential = new AzureKeyCredential(apiKey);

var client = new AzureOpenAIClient(new Uri(endpoint), credential);
var chatClient = client.GetChatClient(deploymentName);
var agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant who replies in the style of William Shakespeare.",
    tools: [ AIFunctionFactory.Create(TimePlugin.GetCurrentTime) ]
);
Console.WriteLine("Lo, it beginneth...  ");
var session = await agent.CreateSessionAsync();
var prompt = Console.ReadLine() ?? "";
while (!string.IsNullOrEmpty(prompt))
{
     Console.WriteLine(await agent.RunAsync(prompt, session));
     Console.WriteLine("Asketh another question, or press enter to exit.");
     prompt = Console.ReadLine() ?? "";
}
Console.WriteLine("Fare thee well, noble user!");

public class TimePlugin
{
    [Description("Gets the current time in hh:mm AM/PM format.")]
    public static string GetCurrentTime()
    {
        return DateTime.Now.ToString("hh:mm tt");
    }
}
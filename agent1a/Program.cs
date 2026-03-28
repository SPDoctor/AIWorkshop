using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

#pragma warning disable OPENAI001

const string deploymentName = "gpt-5.4";
const string endpoint = "https://billa-workshop-resource.openai.azure.com/openai/v1/";
string apiKey = Environment.GetEnvironmentVariable("apiKey_gpt-5.4", EnvironmentVariableTarget.User) ?? "";

 
ChatClient client = new(
    credential: new ApiKeyCredential(apiKey),
    model: deploymentName,
    options: new OpenAIClientOptions()
    {
        Endpoint = new($"{endpoint}"),
    }); 

string question = Console.ReadLine() ?? "";
ChatCompletion completion = client.CompleteChat(
     [
         new SystemChatMessage("You are an AI assistant that helps people find information. Always answer in the form of a question, as in the game jeopardy."),
         new UserChatMessage(question),
     ]);

Console.WriteLine($"Model={completion.Model}");
foreach (ChatMessageContentPart contentPart in completion.Content)
{
    string message = contentPart.Text;
    Console.WriteLine($"Chat Role: {completion.Role}");
    Console.WriteLine("Message:");
    Console.WriteLine(message);
}

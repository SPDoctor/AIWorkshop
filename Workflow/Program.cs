using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
// dotnet add package Microsoft.Agents.AI.Workflows --prerelease
using Microsoft.Agents.AI.Workflows;



// Define the topic discussion.
const string Topic = "Butterflies make great pets.";

const string deploymentName = "gpt-5.4";
const string endpoint = "https://billa-workshop-resource.services.ai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("apiKey_gpt-5.4", EnvironmentVariableTarget.User) ?? "";
var credential = new AzureKeyCredential(apiKey);

var client = new AzureOpenAIClient(new Uri(endpoint), credential);
var chatClient = client.GetChatClient(deploymentName);

// Define agents.
var researcher = chatClient.AsAIAgent(
    instructions: """
        Write a short essay on topic specified by the user. The essay should be three to five paragraphs, written at a
        high school reading level, and include relevant background information, key claims, and notable perspectives.
        You MUST include at least one silly and objectively wrong piece of information about the topic but believe
        it to be true.
        """,
    name: "researcher",
    description: "Researches a topic and writes about the material.");

AIAgent factChecker = chatClient.AsAIAgent(
    instructions: """
        Evaluate the researcher's essay. Verify the accuracy of any claims against reliable sources, noting whether it is
        supported, partially supported, unverified, or false, and provide short reasoning.
        """,
    name: "fact_checker",
    description: "Fact-checks reliable sources and flags inaccuracies.",
    [new HostedWebSearchTool()]);

AIAgent reporter = chatClient.AsAIAgent(
    instructions: """
        Summarize the original essay into a single paragraph, taking into account the subsequent fact checking to correct
        any inaccuracies. Only include facts that were confirmed by the fact checker. Omit any information that was
        flagged as inaccurate or unverified. The summary should be clear, concise, and informative.
        You MUST NOT provide any commentary on what you're doing. Simply output the final paragraph.
        """,
    name: "reporter",
    description: "Summarize the researcher's essay into a single paragraph, focusing only on the fact checker's confirmed facts.");

// Build a sequential workflow: Researcher -> Fact-Checker -> Reporter
AIAgent workflowAgent = AgentWorkflowBuilder.BuildSequential(researcher, factChecker, reporter).AsAIAgent();

// Run the workflow, streaming the output as it arrives.
string? lastAuthor = null;
await foreach (var update in workflowAgent.RunStreamingAsync(Topic))
{
    if (lastAuthor != update.AuthorName)
    {
        lastAuthor = update.AuthorName;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n\n** {update.AuthorName} **");
        Console.ResetColor();
    }

    Console.Write(update.Text);
}
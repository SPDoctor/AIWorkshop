// dotnet add package Azure.AI.Projects --version 2.0.0-beta.1
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using OpenAI;
using OpenAI.Responses;

#pragma warning disable OPENAI001

const string projectEndpoint = "https://billa-workshop-resource.services.ai.azure.com/api/projects/billa-workshop";
const string agentName = "agent1";

// Connect to your project using the endpoint from your project page
// The AzureCliCredential will use your logged-in Azure CLI identity, make sure to run `az login` first
AIProjectClient projectClient = new(endpoint: new Uri(projectEndpoint), tokenProvider: new DefaultAzureCredential());

ProjectResponsesClient responseClient = projectClient.OpenAI.GetProjectResponsesClientForAgent(agentName);
// Use the agent to generate a response
ResponseResult response = responseClient.CreateResponse(
    "What is the capital of France?"
);

Console.WriteLine(response.GetOutputText());

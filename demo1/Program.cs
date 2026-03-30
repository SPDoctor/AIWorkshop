// dotnet add package Azure.AI.Projects --version 2.0.0-beta.2
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI.Responses;

#pragma warning disable OPENAI001

const string projectEndpoint = "https://billa-workshop-resource.services.ai.azure.com/api/projects/billa-workshop";
const string agentName = "agent1";
const string agentVersion = "1";

// Connect to your project using the endpoint from your project page
// The AzureCliCredential will use your logged-in Azure CLI identity, make sure to run `az login` first
AIProjectClient projectClient = new(endpoint: new Uri(projectEndpoint), tokenProvider: new DefaultAzureCredential());

AgentReference agentReference = new(name: agentName, version: agentVersion);
ProjectResponsesClient responseClient = projectClient.OpenAI.GetProjectResponsesClientForAgent(agentReference);
// Use the agent to generate a response
ResponseResult response = responseClient.CreateResponse(
    "Hello! Tell me a joke."
);

Console.WriteLine(response.GetOutputText());

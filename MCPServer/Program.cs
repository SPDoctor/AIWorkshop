using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<MathsTools>();

await builder.Build().RunAsync();

internal class MathsTools
{
  [McpServerTool]
  [Description("Generates a random number between the specified minimum and maximum values.")]
  public int GetRandomNumber(
      [Description("Minimum value (inclusive)")] int min = 0,
      [Description("Maximum value (exclusive)")] int max = 100)
  {
    return Random.Shared.Next(min, max);
  }
}

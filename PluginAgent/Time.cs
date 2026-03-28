using System.ComponentModel;

namespace Plugins;

public class TimePlugin
{
  [Description("Get the current date and time")]
  public static DateTime Time()
  {
    return DateTime.UtcNow;
  }
}
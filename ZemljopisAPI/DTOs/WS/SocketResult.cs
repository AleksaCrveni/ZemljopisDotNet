using Newtonsoft.Json;

namespace ZemljopisAPI.DTOs.WS;

public class SocketResult
{
  public Events Evt { get; set; }
  public bool Success { get; set; } = true;
  public object Response { get; set; } = "";
  [JsonIgnore]
  public bool Disconnect { get; set; } = false;
}
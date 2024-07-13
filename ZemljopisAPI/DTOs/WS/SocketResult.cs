namespace ZemljopisAPI.DTOs.WS;

public class SocketResult
{
  public Events Evt { get; set; }
  public bool Success { get; set; } = true;
  public string ResponseString { get; set; } = "";
}
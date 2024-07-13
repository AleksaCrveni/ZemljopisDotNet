namespace ZemljopisAPI.Services.Sockets;

public interface ISocketService
{
  // @TODO think about string or buffer return
  public Task<string> ProcessSocketData(string socketData);
}
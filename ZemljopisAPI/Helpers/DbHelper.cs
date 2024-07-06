using StackExchange.Redis;
namespace ZemljopisAPI.Helpers;

public sealed class DbHelper
{
  private static ConnectionMultiplexer instance = null;
  private static readonly object padlock = new object();

  DbHelper()
  {
  }

  public static ConnectionMultiplexer Init
  {
    get
    {
      lock (padlock)
      {
        if (instance == null)
        {
          instance = ConnectionMultiplexer.Connect("localhost:6379");
          var db = instance.GetDatabase();
          var res = db.Ping();
          db.StringSet("kurac","s");
        }
        return instance;
      }
    }
  }
}
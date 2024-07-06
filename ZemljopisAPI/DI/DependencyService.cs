using System.Reflection;

namespace ZemljopisAPI.DI;

public static class DependencyService
{
  public static IServiceCollection AddDependencies(this IServiceCollection services)
  {
    var baseType = typeof(IPrivateDependency);
    var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
    var getFiles = Directory.GetFiles(path, "*.dll");
    var referencedAssembiles = getFiles.Select(Assembly.LoadFrom).ToList();

    var types = referencedAssembiles
      .SelectMany(a => a.DefinedTypes)
      .Select(type => type.AsType())
      .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToList();

    var implementTypes = types.Where(x => x.IsClass).ToList();

    foreach (var implementType in implementTypes)
    {
      var canInjectInserfaces = implementType.GetInterfaces()
        .Where(u => u != typeof(IPrivateDependency) && !typeof(IPrivateDependency).IsAssignableFrom((u)));

      AddDependencies(services, implementType, canInjectInserfaces);
    }
    return services;
  }
  public static void AddDependencies(this IServiceCollection services, Type implementType, IEnumerable<Type> canInjectInterfaces)
  {
    if (typeof(IScoped).IsAssignableFrom(implementType))
    {
      foreach (var canInjectInterface in canInjectInterfaces)
      {
        services.AddScoped(canInjectInterface, implementType);
      }
    }
    else if (typeof(ISingleton).IsAssignableFrom(implementType))
    {
      foreach (var canInjectInterface in canInjectInterfaces)
      {
        services.AddSingleton(canInjectInterface, implementType);

      }
    }
    else if (typeof(ITransient).IsAssignableFrom(implementType))
    {
      foreach (var canInjectInterface in canInjectInterfaces)
      {
        services.AddTransient(canInjectInterface, implementType);

      }
    }
  }
}
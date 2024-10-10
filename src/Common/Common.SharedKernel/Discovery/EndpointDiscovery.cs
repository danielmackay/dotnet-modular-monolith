using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace Common.SharedKernel.Discovery;

public static class EndpointDiscovery
{
    private static readonly Type _endpointType = typeof(IEndpoint);

    public static void DiscoverEndpoints(this IEndpointRouteBuilder builder, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));

        var moduleTypes = GetModuleTypes(assemblies);

        foreach (var type in moduleTypes)
        {
            var obj = Activator.CreateInstance(type);
            var method = GetMapEndpointMethod(type);
            method?.Invoke(obj, [builder]);
        }
    }
    
    private static IEnumerable<Type> GetModuleTypes(params Assembly[] assemblies) =>
        assemblies.SelectMany(x => x.GetTypes())
            .Where(x => _endpointType.IsAssignableFrom(x) &&
                        x is { IsInterface: false, IsAbstract: false });

    private static MethodInfo? GetMapEndpointMethod(Type type) =>
        type.GetMethod(nameof(IEndpoint.MapEndpoint));
}
using System.Reflection;

namespace WebAPI.Annotation;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class RepositoryAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute { }

public static class AnnotationExtension
{
    public static IServiceCollection AddAnnotation(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        foreach (var assesembly in assemblies)
        {
            var types = assesembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract &&
                                                        (t.GetCustomAttribute<ServiceAttribute>() != null ||
                                                         t.GetCustomAttribute<RepositoryAttribute>() != null ||
                                                         t.GetCustomAttribute<ComponentAttribute>() != null ||
                                                         t.GetCustomAttribute<ConfigurationAttribute>() != null)
            );

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Any())
                {
                    foreach (var iface in interfaces)
                    {
                        services.AddScoped(iface, type);
                    }
                }
                else
                {
                    services.AddScoped(type);
                }
            }
        }
        return services;
    }
}
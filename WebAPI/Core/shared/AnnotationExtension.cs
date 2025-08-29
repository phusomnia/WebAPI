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

[AttributeUsage(AttributeTargets.Class)]
public class ProviderAttribute : Attribute { }

public static class AnnotationExtension
{
    public static IServiceCollection AddAnnotation(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        var attributeTypes = new Type[]
        {
            typeof(ServiceAttribute),
            typeof(RepositoryAttribute),
            typeof(ComponentAttribute),
            typeof(ConfigurationAttribute),
            typeof(ProviderAttribute)
        };

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => attributeTypes.Any(attrType => t.GetCustomAttribute(attrType) != null));

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Any())
                {
                    if (type.GetCustomAttribute<ProviderAttribute>() != null)
                    {
                        foreach (var iface in interfaces)
                        {
                            services.AddSingleton(iface, type);
                        }
                    }
                    else
                    {
                        foreach (var iface in interfaces)
                        {
                            services.AddScoped(iface, type);
                        }
                    }
                }
                services.AddScoped(type);
            }
        }
        return services;
    }
}
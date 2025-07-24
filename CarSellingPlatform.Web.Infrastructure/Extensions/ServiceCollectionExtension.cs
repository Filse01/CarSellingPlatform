using System.Reflection;
using CarSellingPlatform.Web.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CarSellingPlatform.Web.Infrastructure.Extensions;
using static GCommon.ExceptionMessages;
public static class ServiceCollectionExtension
{
    public static readonly string ProjectInterfacePrefix = "I";
    public static readonly string ServiceTypeSuffix = "Service";
    public static IServiceCollection RegisterUserDefinedServices(this IServiceCollection services, Assembly assembly)
    {
        Type[] serviceClasses = assembly.GetTypes()
            .Where(t => !t.IsInterface && t.Name.EndsWith(ServiceTypeSuffix))
            .ToArray();
        foreach (Type serviceClass in serviceClasses)
        {
            Type? serviceInterface = serviceClass.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"{ProjectInterfacePrefix}{serviceClass.Name}");
            if (serviceInterface == null)
            {
                throw new ArgumentException(InterfaceNotFoundMessage, serviceClass.Name);
            }
            services.AddScoped(serviceInterface, serviceClass);
        }
        return services;
    }

    public static IApplicationBuilder UserAdminRedirection(this IApplicationBuilder app)
    {
        app.UseMiddleware<AdminRedirectionMiddleware>();
        return app;
    }
}
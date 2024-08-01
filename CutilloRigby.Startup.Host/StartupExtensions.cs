using CutilloRigby.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    public static class StartupExtensions
    {
        public static void ConfigurationConfigureFromAssemblyContaining<TMarker>(this IConfiguration configuration,
            IHostEnvironment environment)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblyContaining<TMarker, IConfigurationConfigure>(
                x => x.Configure(environment, configuration));
        }

        public static void ConfigurationConfigureFromAssembliesContaining(this IConfiguration configuration,
            IHostEnvironment environment, params Type[] assemblyMarkers)
        {
            StartupHelpers.RunCustomStartupActionsFromAssembliesContaining<IConfigurationConfigure>(
                x => x.Configure(environment, configuration), assemblyMarkers);
        }

        public static void ConfigurationConfigureFromAssemblies(this IConfiguration configuration,
            IHostEnvironment environment, params Assembly[] assemblies)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblies<IConfigurationConfigure>(
                x => x.Configure(environment, configuration), assemblies);
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static void ConfigureServicesFromAssemblyContaining<TMarker>(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblyContaining<TMarker, IConfigureServices>(
                x => x.ConfigureServices(serviceCollection, configuration));
        }

        public static void ConfigureServicesFromAssembliesContaining(this IServiceCollection serviceCollection,
            IConfiguration configuration, params Type[] assemblyMarkers)
        {
            StartupHelpers.RunCustomStartupActionsFromAssembliesContaining<IConfigureServices>(
                x => x.ConfigureServices(serviceCollection, configuration), assemblyMarkers);
        }

        public static void ConfigureServicesFromAssemblies(this IServiceCollection serviceCollection,
            IConfiguration configuration, params Assembly[] assemblies)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblies<IConfigureServices>(
                x => x.ConfigureServices(serviceCollection, configuration), assemblies);
        }
    }
}

namespace Microsoft.Extensions.Hosting
{
    public static class StartupExtensions
    {
        public static void ConfigureFromAssemblyContaining<TMarker>(this IHostBuilder hostBuilder)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblyContaining<TMarker, IHostConfigure>(
                x => x.Configure(hostBuilder));
        }

        public static void ConfigureFromAssembliesContaining(this IHostBuilder hostBuilder,
            params Type[] assemblyMarkers)
        {
            StartupHelpers.RunCustomStartupActionsFromAssembliesContaining<IHostConfigure>(
                x => x.Configure(hostBuilder), assemblyMarkers);
        }

        public static void ConfigureFromAssemblies(this IHostBuilder hostBuilder, params Assembly[] assemblies)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblies<IHostConfigure>(
                x => x.Configure(hostBuilder), assemblies);
        }

        public static void PostBuildConfigureFromAssemblyContaining<TMarker>(this IHost host,
            IServiceScope serviceScope)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblyContaining<TMarker, IPostBuildConfigure>(
                x => x.PostBuildConfigure(host, serviceScope));
        }

        public static void PostBuildConfigureFromAssembliesContaining(this IHost host, IServiceScope serviceScope,
            params Type[] assemblyMarkers)
        {
            StartupHelpers.RunCustomStartupActionsFromAssembliesContaining<IPostBuildConfigure>(
                x => x.PostBuildConfigure(host, serviceScope), assemblyMarkers);
        }

        public static void PostBuildConfigureFromAssemblies(this IHost host, IServiceScope serviceScope,
            params Assembly[] assemblies)
        {
            StartupHelpers.RunCustomStartupActionsFromAssemblies<IPostBuildConfigure>(
                x => x.PostBuildConfigure(host, serviceScope), assemblies);
        }

        public static IHost PostBuildConfigure(this IHost host, Action<IHost, IServiceScope> postBuildConfigure)
        {
            using var serviceScope = host.Services.CreateScope();
            postBuildConfigure(host, serviceScope);
            return host;
        }
    }
}

namespace System.Reflection
{
    public static class StartupExtensions
    {
        public static IEnumerable<TStartup> GetStartupTypesFromAssemblyContaining<TMarker, TStartup>()
            where TStartup : IStartup
        {
            return GetStartupTypesFromAssembly<TStartup>(typeof(TMarker).Assembly);
        }

        public static IEnumerable<TStartup> GetStartupTypesFromAssembly<TStartup>(this Assembly assembly)
            where TStartup : IStartup
        {
            var tType = typeof(TStartup);

            return assembly.DefinedTypes
                .Where(x => tType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<TStartup>()
                .OrderBy(x => x.Order);
        }

        public static string GetTitleFromAssemblyContaining<TMarker>()
        {
            return GetTitleFromAssembly(typeof(TMarker).Assembly);
        }

        public static string GetTitleFromAssembly(this Assembly assembly)
        {
            var asmName = assembly.GetName();
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var name = titleAttr?.Title ?? asmName.Name;

            return $"{name} v{asmName.Version}";
        }

        public static string GetClientNameFromAssemblyContaining<TMarker>()
        {
            return GetClientNameFromAssembly(typeof(TMarker).Assembly);
        }

        public static string GetClientNameFromAssembly(this Assembly assembly)
        {
            var asmName = assembly.GetName();
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var name = titleAttr?.Title ?? asmName.Name;

            return name.Replace(" ", "");
        }

        public static string GetUserAgentStringFromAssemblyContaining<TMarker>()
        {
            return GetUserAgentStringFromAssembly(typeof(TMarker).Assembly);
        }

        public static string GetUserAgentStringFromAssembly(this Assembly assembly)
        {
            var asmName = assembly.GetName();
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var name = titleAttr?.Title ?? asmName.Name;

            return $"{name.Replace(" ", "")}/v{asmName.Version}";
        }

        public static string GetShortName(this Assembly assembly)
        {
            var name = assembly.FullName;
            var index = name.IndexOf(',');
            return index < 0 ? name : name[..index];
        }
    }
}
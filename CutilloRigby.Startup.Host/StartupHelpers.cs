using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CutilloRigby.Startup;

public static class StartupHelpers
{
    public static void RunCustomStartupActionsFromAssemblyContaining<TMarker, TStartup>(Action<TStartup> action)
        where TStartup : IStartup
    {
        RunCustomStartupActionsFromAssembliesContaining(action, typeof(TMarker));
    }

    public static void RunCustomStartupActionsFromAssembliesContaining<TStartup>(Action<TStartup> action, params Type[] assemblyMarkers)
        where TStartup : IStartup
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).ToArray();
        RunCustomStartupActionsFromAssemblies(action, assemblies);
    }

    public static void RunCustomStartupActionsFromAssemblies<TStartup>(Action<TStartup> action, params Assembly[] assemblies)
        where TStartup : IStartup
    {
        var logger = GetLogger();
        var startupTypeName = typeof(TStartup).Name;

        foreach (var assembly in assemblies)
        {
            var assemblyName = assembly.GetShortName();
            var startups = assembly.GetStartupTypesFromAssembly<TStartup>();

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Started {StartupTypeName} AutoStartup in assembly {AssemblyName}.", 
                    startupTypeName, assemblyName);

            foreach (var startup in startups)
            {
                var startupName = startup.GetType().Name;

                try
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation("Running {StartupTypeName} action for {StartupName}.", 
                            startupTypeName, startupName);
                    action(startup);
                }
                catch (Exception _ex)
                {
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(_ex, "Error with {StartupTypeName} action for {StartupName}.",
                            startupTypeName, startupName);
                }
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Completed {StartupTypeName} AutoStartup in assembly {AssemblyName}.",
                    startupTypeName, assemblyName);
        }
    }

    private static ILogger GetLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddSystemdConsole(configure =>
            {
                configure.IncludeScopes = false;
                configure.TimestampFormat = "HH:mm:ss";
            });
        });

        return loggerFactory.CreateLogger("");
    }
}

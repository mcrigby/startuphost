using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

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
        Action<string, string> logStart = (startupTypeName, assemblyName) => { };
        Action<string, string> logRun = (startupTypeName, startupName) => { };
        Action<Exception, string, string> logError = (ex, startupTypeName, startupName) => { };
        Action<string, string, long> logComplete = (startupTypeName, assemblyName, elapsedMs) => { };

        var logger = GetLogger();
        if (logger.IsEnabled(LogLevel.Information))
        {
            logStart = (startupTypeName, assemblyName) => logger.LogInformation(
                "Started {StartupTypeName} AutoStartup in assembly {AssemblyName}.", startupTypeName, assemblyName);
            logRun = (startupTypeName, startupName) => logger.LogInformation(
                "Running {StartupTypeName} action for {StartupName}.", startupTypeName, startupName);
            logComplete = (startupTypeName, assemblyName, elapsedMs) => logger.LogInformation(
                "Completed {StartupTypeName} AutoStartup in assembly {AssemblyName} in {Elapsed_ms}ms.",
                    startupTypeName, assemblyName, elapsedMs);
        }

        if (logger.IsEnabled(LogLevel.Error))
            logError = (ex, startupTypeName, startupName) => logger.LogError(ex,
                "Error with {StartupTypeName} action for {StartupName}.", startupTypeName, startupName);

        var startupTypeName = typeof(TStartup).Name;
        foreach (var assembly in assemblies
            .Distinct())
        {
            var assemblyName = assembly.GetShortName();
            var startups = assembly.GetStartupTypesFromAssembly<TStartup>();

            logStart(startupTypeName, assemblyName);
            var stopWatch = Stopwatch.StartNew();

            foreach (var startup in startups)
            {
                var startupName = startup.GetType().Name;

                try
                {
                    logRun(startupTypeName, startupName);
                    action(startup);
                }
                catch (Exception _ex)
                {
                    logError(_ex, startupTypeName, startupName);
                }
            }

            stopWatch.Stop();
            logComplete(startupTypeName, assemblyName, stopWatch.ElapsedMilliseconds);
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

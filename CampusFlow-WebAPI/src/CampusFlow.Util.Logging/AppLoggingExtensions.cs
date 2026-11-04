using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CampusFlow.Util.Logging;

public static class AppLoggingExtensions
{
    public static IServiceCollection AddAppLogger(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var options = configuration.GetSection(AppLoggingOptions.SectionName).Get<AppLoggingOptions>()
            ?? new AppLoggingOptions();

        switch (options.Type)
        {
            case LoggerType.File:
                var path = Path.Combine(environment.ContentRootPath, options.FilePath);
                services.AddSingleton<IFileWriter>(new FileWriter(path));
                services.AddSingleton<ILogSink, FileLogSink>();
                break;

            case LoggerType.Console:
                services.AddSingleton<ILogSink, ConsoleLogSink>();
                break;

            case LoggerType.Debug:
                services.AddSingleton<ILogSink, DebugLogSink>();
                break;

            default:
                throw new NotSupportedException($"The logger type '{options.Type}' is not supported.");
        }

        services.AddSingleton<IAppLogger, AppLogger>();
        return services;
    }
}

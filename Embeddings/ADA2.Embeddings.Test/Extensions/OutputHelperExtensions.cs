using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test.Extensions;

public static class OutputHelperExtensions
{
    public static ILogger GetLogger(this ITestOutputHelper output, LogEventLevel level = LogEventLevel.Information)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Is(level)
            .CreateLogger();
        
        return Log.Logger;
    }
}

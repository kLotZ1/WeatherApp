using Serilog;

namespace WeatherApp.Api;

public static class RegistrationExtensions
{
    public static IHostBuilder RegisterSerilog(this IHostBuilder webApplicationBuilder)
    {
        return webApplicationBuilder.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }
}
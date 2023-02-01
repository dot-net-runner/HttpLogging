using Microsoft.AspNetCore.Builder;
using DotNetRunner.HttpLogging.Middlewares;
using DotNetRunner.HttpLogging.Models;

namespace DotNetRunner.HttpLogging.Configuring;

public static class HttpLoggingBuilderExtension
{
    /// <summary>
    /// Добавить логирование http запросов
    /// </summary>
    public static IApplicationBuilder UseHttpRequestsLogging(this IApplicationBuilder app,
        Action<HttpLoggingOptions> configuration = null)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var options = new HttpLoggingOptions();
        configuration?.Invoke(options);

        app.UseMiddleware<HttpLoggingMiddleware>(options);
        return app;
    }
}
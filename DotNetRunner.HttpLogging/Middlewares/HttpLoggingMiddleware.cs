using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using DotNetRunner.HttpLogging.Models;

namespace DotNetRunner.HttpLogging.Middlewares;

internal class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpLoggingMiddleware> _logger;
    private readonly HttpLoggingOptions _options;

    public HttpLoggingMiddleware(RequestDelegate next,
        ILogger<HttpLoggingMiddleware> logger,
        HttpLoggingOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (IgnoreRequest(context))
        {
            await _next(context);
            return;
        }

        var headers = ExtractHeaders(context.Request.Headers);

        var requestBody = await ExtractRequestBodyAsync(context.Request);

        var originalBodyStream = context.Response.Body;

        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var elapsed = await ExecuteNextAsync(context);

        var responseBody = await ExtractResponseBodyAsync(context.Response);

        var (formattedRequestBody, formattedResponseBody) =
            FormatBodies(requestBody, responseBody, context.Request);

        var request = new HttpRequestLogEntry(context.Request.Method,
            context.Request.GetDisplayUrl(),
            formattedRequestBody,
            headers);

        var response = new HttpResponseLogEntry(context.Response.StatusCode, formattedResponseBody);

        _logger.LogInformation("{request} {response} {elapsed}",
            JsonSerializer.Serialize(request, _options.SerializerOptions),
            JsonSerializer.Serialize(response, _options.SerializerOptions),
            elapsed);

        await responseStream.CopyToAsync(originalBodyStream);
    }

    private async Task<string> ExtractRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        return body.Length > _options.RequestBodyLimit
            ? body.Substring(0, _options.RequestBodyLimit.Value)
            : body;
    }

    private async Task<string> ExtractResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return body;
    }


    private bool IgnoreRequest(HttpContext context)
    {
        return _options.IgnoredPathPatterns
            .Any(p => Regex.IsMatch(context.Request.Path.ToString(), p));
    }

    private async Task<TimeSpan> ExecuteNextAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        return stopwatch.Elapsed;
    }

    private IDictionary<string, string> ExtractHeaders(IHeaderDictionary headers)
    {
        var filteredHeaders = headers
            .Where(h => !_options.IgnoredHeaderPatterns
                .Any(p => Regex.IsMatch(h.Key, p)));

        return filteredHeaders
            .ToDictionary(h => h.Key,
                h => h.Value.ToString());
    }

    private (string requestBody, string responseBody) FormatBodies(string requestBody, string responseBody,
        HttpRequest request)
    {
        var formatter = _options.BodyFormatters
            .FirstOrDefault(p => p.Pattern.IsMatch(request));

        string formattedRequestBody = null;
        string formattedResponseBody = null;
        if (formatter is not null)
        {
            formatter.RequestBodyFormatter?.TryFormate(requestBody, out formattedRequestBody);
            formatter.ResponseBodyFormatter?.TryFormate(responseBody, out formattedResponseBody);
        }

        formattedRequestBody ??= requestBody;
        formattedResponseBody ??= responseBody;

        return (formattedRequestBody, formattedResponseBody);
    }
}
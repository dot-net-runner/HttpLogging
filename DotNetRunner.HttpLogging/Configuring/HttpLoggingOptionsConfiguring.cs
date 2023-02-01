using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetRunner.HttpLogging.BodyFormat;
using DotNetRunner.HttpLogging.Models;

namespace DotNetRunner.HttpLogging.Configuring;

public static class HttpLoggingOptionsConfiguring
{
    public static HttpLoggingOptions IgnoreSwagger(this HttpLoggingOptions options)
    {
        options.IgnoredPathPatterns.Add("/swagger.*");

        return options;
    }

    public static HttpLoggingOptions CreateDefaultSerializer(this HttpLoggingOptions options)
    {
        options.SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return options;
    }

    public static HttpLoggingOptions IgnoreAllHeaders(this HttpLoggingOptions options)
    {
        options.IgnoredHeaderPatterns.Add(".*");

        return options;
    }

    public static HttpLoggingOptions AddBodyFormatter(this HttpLoggingOptions options, IgnorePattern pattern,
        IBodyFormatter requestFormatter = null,
        IBodyFormatter responseFormatter = null)
    {
        options.BodyFormatters.Add(new PatternBodyFormatter(pattern, requestFormatter, responseFormatter));
        return options;
    }
}
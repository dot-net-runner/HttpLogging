using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace DotNetRunner.HttpLogging.BodyFormat;

public class IgnorePattern
{
    public string HttpMethod { get; }
    public Regex UrlPattern { get; }
    
    
    public IgnorePattern(string httpMethod, string urlPattern)
    {
        HttpMethod = httpMethod;
        UrlPattern = new (urlPattern);
    }

    public bool IsMatch(HttpRequest request)
    {
        return UrlPattern.IsMatch(request.Path.ToString())
               && HttpMethod == request.Method;
    }
}
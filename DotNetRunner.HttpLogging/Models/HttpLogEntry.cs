namespace DotNetRunner.HttpLogging.Models;

internal record HttpRequestLogEntry(string method, string uri, string body, IDictionary<string, string> headers);

internal record HttpResponseLogEntry(int? code, string body);
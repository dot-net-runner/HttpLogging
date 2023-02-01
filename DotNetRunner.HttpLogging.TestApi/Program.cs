using DotNetRunner.HttpLogging.BodyFormat;
using DotNetRunner.HttpLogging.BodyFormat.Json;
using DotNetRunner.HttpLogging.Configuring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpRequestsLogging(options =>
{
    options.IgnoreSwagger().CreateDefaultSerializer().IgnoreAllHeaders();
    options.AddBodyFormatter(new IgnorePattern("POST", ".*"),
        new JsonPropertyIgnoreFormatter("Content.data", "Content.Error", "NoMatter", "Content.Hello"));
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.Run(async (context) =>
{
    using var sr = new StreamReader(context.Request.Body);
    var body = await sr.ReadToEndAsync();
    await context.Response.WriteAsync(body);
});

app.Run();
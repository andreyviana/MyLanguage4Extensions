
using Microsoft.AspNetCore.Mvc;
using MyLangs4Extensions.Models;
using Newtonsoft.Json;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

HttpClient client = new();

async Task<string> GetExternalExtensions(string url)
{
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    HttpResponseMessage response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadAsStringAsync();
    return result;
}

List<T> ExtensionsJson<T>(string content)
{
    List<T>? extensions = JsonConvert.DeserializeObject<List<T>>(content);
    return extensions;
}

List<Extension> RemoveUnwantedLangs(List<Extension> extensions, string[] wantedLangs)
{
    List<Extension> newExtensions = [];

    foreach (var extension in extensions)
    {
        if (wantedLangs.Contains(extension.Lang))
        {
            extension.Sources = extension.Sources.Where(x => wantedLangs.Contains(x.Lang)).ToList();
            newExtensions.Add(extension);
        }
    }

    return newExtensions;
}

List<ExtensionExtended> RemoveUnwantedLangsExtended(List<ExtensionExtended> extensions, string[] wantedLangs)
{
    List<ExtensionExtended> newExtensions = [];

    foreach (var extension in extensions)
    {
        if (wantedLangs.Contains(extension.Lang))
        {
            extension.Sources = extension.Sources.Where(source => wantedLangs.Contains(source.Lang)).ToList();
            newExtensions.Add(extension);
        }
    }

    return newExtensions;
}

app.MapGet("/extensions", async (string url,[FromQuery] params string[] lang) =>
{
    var result = await GetExternalExtensions(url);
    return RemoveUnwantedLangs(ExtensionsJson<Extension>(result), lang);
})
.WithName("GetContent")
.WithOpenApi();

app.MapGet("/extensions-extended", async (string url, [FromQuery] params string[] lang) =>
{
    var result = await GetExternalExtensions(url);
    return RemoveUnwantedLangsExtended(ExtensionsJson<ExtensionExtended>(result), lang);
})
.WithName("GetContentExtended")
.WithOpenApi();

app.Run();
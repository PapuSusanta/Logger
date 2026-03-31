using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLoggerExtension(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("/", () =>
{
    throw new Exception("An error occurred while processing the request.");
});

await app.RunAsync();
using Microsoft.AspNetCore.Mvc;
using Modmail.NET.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddHostedService<BotService>(); //Discord bot service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health")
   .WithOpenApi(); //TODO: add health check for discord api

app.MapGet("/",
           context => context.Response.WriteAsJsonAsync(new OkObjectResult("Modmail.NET API Service")))
   .WithOpenApi();


//TODO: Add Endpoints:
/*
 * GetModmails (Paginated)
 * CloseModmail (Close a modmail)
 * GetModmailMessages (Paginated)
 * GetMessageEmbed (Get a message embed)
 * AuthenticateByDiscord
 * ReAuthenticateByDiscord
 */


app.Run();
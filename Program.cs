
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}
app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
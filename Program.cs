var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddCors()
    .AddRateLimiting()
    .AddNotificationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseCors("RiverLinePolicy");
app.UseAuthentication();
app.UseRateLimiter(); 
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();
app.MapGet("/debug/ip", (HttpContext ctx) => new
{
    remote = ctx.Connection.RemoteIpAddress?.ToString(),
    forwardedFor = ctx.Request.Headers["X-Forwarded-For"].ToString()
});
app.Run();
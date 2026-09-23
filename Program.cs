var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddCors()
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
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();

app.Run();
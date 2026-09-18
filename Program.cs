

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


    await app.SeedDevelopmentDataAsync();
}
app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseCors("RiverLinePolicy"); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
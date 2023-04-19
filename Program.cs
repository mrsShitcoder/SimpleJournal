using Journal.Models;
using Journal.Services;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JournalDatabaseSettings>(builder.Configuration.GetSection("JournalDatabase"));
builder.Services.AddSingleton<CacheByUser>(serviceProvider => new CacheByUser(100000, TimeSpan.FromMinutes(10)));
builder.Services.AddSingleton<CacheByMessage>(serviceProvider => new CacheByMessage(100000, TimeSpan.FromMinutes(10)));
builder.Services.AddSingleton<JournalDatabaseService>();
builder.Services.AddSingleton<JournalService>();
builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    options.RoutePrefix = string.Empty;
});
app.Run();
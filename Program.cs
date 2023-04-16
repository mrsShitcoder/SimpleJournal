using Journal.Models;
using Journal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JournalDatabaseSettings>(builder.Configuration.GetSection("JournalDatabase"));
builder.Services.AddSingleton<PreviewsCache>(serviceProvider => new PreviewsCache(1000, TimeSpan.FromHours(1)));
builder.Services.AddSingleton<ContentsCache>(serviceProvider => new ContentsCache(100000, TimeSpan.FromHours(1)));
builder.Services.AddSingleton<JournalDatabaseService>();
builder.Services.AddSingleton<JournalService>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
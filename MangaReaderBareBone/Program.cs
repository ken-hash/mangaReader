using MangaReaderBareBone.Data;
using MangaReaderBareBone.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MangaReaderBareBoneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MangaReaderBareBoneContext") ?? throw new InvalidOperationException("Connection string 'MangaReaderBareBoneContext' not found.")));

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SectionName));
builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RateLimitOptions>>().Value);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowCORS", builder =>
    {
        builder.WithOrigins(
            "http://localhost:5000",
            "http://localhost:5001")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine("\\\\192.168.50.11", "Public-Manga"
    ))
});

app.UseCors("AllowCORS");
builder.Services.AddResponseCaching();
app.UseResponseCaching();
app.UseRouting();
app.UseRateLimiting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

using MangaReaderBareBone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//TODO Disable sql in debugging to accept dummy data
builder.Services.AddDbContext<MangaReaderBareBoneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MangaReaderBareBoneContext") ?? throw new InvalidOperationException("Connection string 'MangaReaderBareBoneContext' not found.")));

// Add services to the container.
builder.Services.AddControllers();
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseDeveloperExceptionPage();

}
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine("\\\\192.168.50.11", "Public-Manga"
    ))
    //EnableDirectoryBrowsing = true
});

app.UseCors("AllowCORS");
//app.UseHttpsRedirection();
builder.Services.AddResponseCaching();
app.UseResponseCaching();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();

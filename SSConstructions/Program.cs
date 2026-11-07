using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Implementation;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ConstructionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConstructionContext")));

builder.Services.Configure<LogSettings>(
    builder.Configuration.GetSection("ConstructionLogFilePath"));
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<CustomLogger>();
builder.Services.AddScoped<IUsers, Users>();
builder.Services.AddScoped<ILoginRepo, LoginRepo>();
builder.Services.AddScoped<IProjectRepo, ProjectRepo>();
builder.Services.AddScoped<IAppConfigRepo, AppConfigRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<IExtraRepo, ExtraRepo>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "addCors",
        builder => {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
var jwt = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JWTSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt.Key))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve runtime uploads (logos, project images) through the static-file middleware
// instead of MapStaticAssets. MapStaticAssets snapshots each file's length at startup,
// so replacing a file with a different size makes it render truncated — or fail with
// a 500 when the new file is smaller. PhysicalFileProvider re-reads the file per request.
var webRoot = app.Environment.WebRootPath
    ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var uploadsRoot = Path.Combine(webRoot, "uploads");
Directory.CreateDirectory(uploadsRoot);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        // Uploaded files are replaced in place, so never let browsers/CDNs cache them.
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        ctx.Context.Response.Headers.Pragma = "no-cache";
        ctx.Context.Response.Headers.Expires = "0";
    }
});

app.UseRouting();

app.UseCors("addCors");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.MapControllers();

// Angular SPA fallback (VERY IMPORTANT – must be last)
app.MapFallbackToFile("index.html");

app.Run();

using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Gauniv.WebServer.Security;
using Gauniv.WebServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Globalization;
using Gauniv.WebServer.Websocket;

var builder = WebApplication.CreateBuilder(args);

// Set the culture so that the culture is the same between front and back
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(s =>
{
    s.SupportedCultures = new[] { cultureInfo };
    s.SupportedUICultures = new[] { cultureInfo };
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity services (user and role management)
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

// Add controllers and Razor pages
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddRazorPages();

// Add OpenAPI for Swagger documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Add AutoMapper and other services
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSignalR();
builder.Services.AddHostedService<OnlineService>();
builder.Services.AddHostedService<SetupService>();

// Add JWT Authentication and Authorization services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Authorization setup (including role-based access for Admins)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddScoped<GameService>();

var app = builder.Build();

// Initialize Admin account if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var adminRole = await roleManager.FindByNameAsync("Admin");
    if (adminRole == null)
    {
        adminRole = new IdentityRole("Admin");
        await roleManager.CreateAsync(adminRole);
    }

    var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            FirstName = "Admin",  // Ajout du prénom
            LastName = "User"     // Ajout du nom de famille
        };
        await userManager.CreateAsync(adminUser, "AdminPassword123!");  // Default password for the admin
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

}

app.UseHttpsRedirection();
app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure static assets
app.MapStaticAssets();

// Default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});

// SignalR setup for real-time communication
app.MapHub<OnlineHub>("/online");

app.Run();

using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// Entry point for configuring and running the web application.
/// This sets up services, middleware, authentication, and other necessary configurations.
/// </summary>
/// <author>Mário Silva - 202000500</author>
/// <author>Luís Martins - 202100239</author>
var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
/// <summary>
/// Add controllers, API documentation (Swagger), and other necessary configurations.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// <summary>
/// Configures the database context to use SQL Server connection.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ICareServerContext>(options =>
    options.UseSqlServer(connectionString));

// Configuração do Identity
/// <summary>
/// Configures Identity services, including password policies, user registration, and token management.
/// </summary>
builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
    })
    .AddEntityFrameworkStores<ICareServerContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

/// <summary>
/// Configures authentication using JWT tokens, setting up token validation parameters, such as issuer, audience, and key.
/// </summary>
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Configuração de serialização JSON
/// <summary>
/// Configures JSON serialization to use string-based enum values in API responses.
/// </summary>
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Configuração de CORS (Cross-Origin Resource Sharing)
/// <summary>
/// Configures CORS to allow specific origins for frontend-backend communication.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://localhost:4200",
            "https://127.0.0.1:4200",
            "https://icaresite.azurewebsites.net"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Evitar redirecionamentos automáticos
/// <summary>
/// Prevents automatic redirection to login pages by returning a 401 Unauthorized status instead.
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

// Serviços personalizados
/// <summary>
/// Registers custom services for logging, email sending, and goal management.
/// </summary>
builder.Services.AddScoped<UserLogService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailSenderService>();
builder.Services.AddScoped<GoalService>();

var app = builder.Build();

// Execução de migrações e seeding de dados
/// <summary>
/// Executes database migrations and seeding for roles and users.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ICareServerContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Aplicar migrações
        //await context.Database.MigrateAsync();

        // Executar seeding
        await RoleSeeder.SeedRoles(roleManager);
        await UserSeeder.SeedUsersAsync(userManager);
    }
    catch (Exception ex)
    {
        // Log do erro
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar migrações ou executar o seeding.");
        throw;
    }
}

// Configuração do pipeline de middleware
/// <summary>
/// Configures middleware to handle exceptions, HTTP redirection, static files, routing, CORS, and authentication.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseRouting();
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores e rotas
/// <summary>
/// Maps controllers and sets up default routing for the application.
/// </summary>
app.MapControllers();
app.MapRazorPages();
app.MapGroup("/api");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
//    context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "credentialless");
//    await next();
//});

// Custom middleware for serving static files or fallback to index page for non-API requests
/// <summary>
/// Middleware for handling non-API requests by redirecting them to the default index page.
/// </summary>
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value != null &&
        !context.Request.Path.Value.StartsWith("/api") &&
        !context.Request.Path.Value.StartsWith("/swagger") &&
        !System.IO.Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
    }
    await next();
});
app.UseStaticFiles();

app.Run();

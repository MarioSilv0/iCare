using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <author>Mário Silva - 202000500</author>
/// <author>Luís Martins - 202100239</author>
/// 
ThreadPool.SetMinThreads(100, 100);
ThreadPool.SetMaxThreads(10000, 10000);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Configuration["ConnectionStrings:Deploy"] =
    builder.Configuration["ConnectionStrings:Deploy"]!.Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "");

builder.Configuration["Jwt:Key"] =
    Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Jwt:Key"];

builder.Configuration["EmailSettings:SenderPassword"] =
    Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? builder.Configuration["EmailSettings:SenderPassword"];

// Configurar Kestrel para permitir mais conexões simultâneas
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});


// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ICareServerContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions
            .CommandTimeout(60)
            .EnableRetryOnFailure())
    );



// Reduz o consumo de CPU no ASP.NET Core
builder.Services.AddResponseCompression();
builder.Services.AddMemoryCache();

// Configuração de serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Identity
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

// Configuração de autenticação e JWT
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
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Configuração de CORS
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
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

// Serviços personalizados
builder.Services.AddScoped<UserLogService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailSenderService>(); 
builder.Services.AddScoped<IGoalService, GoalService>();


var app = builder.Build();

// Execução de migrações e seeding de dados
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

using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

/// <author>Mário Silva - 202000500</author>
/// <author>Luís Martins - 202100239</author>

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ICareServerContext>(options =>
    options.UseSqlServer(connectionString));

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
        policy.WithOrigins("https://localhost:4200")
              .WithOrigins("https://icaresite.azurewebsites.net")
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
        await context.Database.MigrateAsync();

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

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "credentialless");
    await next();
});


app.Use(async (context, next) =>
{
    if (context.Request.Path.Value != null &&
        !context.Request.Path.Value.StartsWith("/api") &&
        !System.IO.Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
    }
    await next();
});
app.UseStaticFiles();


app.Run();

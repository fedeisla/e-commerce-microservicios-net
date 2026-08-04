using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Registrar Redis como Singleton en el contenedor de dependencias
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Registrar nuestro limitador distribuido
builder.Services.AddSingleton<RedisRateLimiter>();

// Configurar la autenticación JWT para que el Gateway sepa leer y validar los tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Habilitar la autorización y políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")); 
});

// Configuración única de YARP leyendo la sección "ReverseProxy" del appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Middleware de Rate Limiting Global usando Redis (Va primero para frenar a los bots en la puerta)
app.Use(async (context, next) =>
{
    var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var rateLimiter = context.RequestServices.GetRequiredService<RedisRateLimiter>();

    // REGLA: Máximo 20 peticiones por minuto por IP a través de todo el Gateway
    bool allowed = await rateLimiter.IsAllowedAsync(clientIp, maxRequests: 20, TimeSpan.FromMinutes(1));

    if (!allowed)
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsync("Demasiadas peticiones. Intente nuevamente en un minuto.");
        return; 
    }

    await next();
});

// Orden crítico de los middlewares de seguridad y ruteo
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
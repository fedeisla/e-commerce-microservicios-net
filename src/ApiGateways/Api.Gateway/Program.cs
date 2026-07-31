var builder = WebApplication.CreateBuilder(args);

//modificar los puertos en base al puerto cuando se compilan los microsevicios

// 1. Agregamos YARP a los servicios y le decimos que lea la configuración del appsettings
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 2. Mapeamos los endpoints (YARP intercepta las peticiones acá)
app.MapReverseProxy();

app.Run();
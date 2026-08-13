using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TicketSoporte.Api.Middelware;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Mapping;
using TicketSoporte.Application.Service;
using TicketSoporte.Domain.Entites;
using TicketSoporte.Infrastructure.Data;
using TicketSoporte.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);


// Cargar variables de entorno desde el archivo .env
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

//leer variables de entorno 
var host = Environment.GetEnvironmentVariable("HOST");
var port = Environment.GetEnvironmentVariable("PORT");
var database = Environment.GetEnvironmentVariable("DATABASE");
var user = Environment.GetEnvironmentVariable("USER");
var password = Environment.GetEnvironmentVariable("PASSWORD");
var key = Environment.GetEnvironmentVariable("JWT_KEY");
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

//construir la cadena de conexion para DB
var connectionString =
    $"Host={host};" +
    $"Port={port};" +
    $"Database={database};" +
    $"Username={user};" +
    $"Password={password};" +
    $"SSL Mode=Require;" +
    $"Trust Server Certificate=true;";




// Registrar el contexto de la base de datos con la cadena de conexión
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


//definir reglas de seguridad
builder.Services.AddIdentity<Usuarios,IdentityRole<int>>( options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


//Registrar Repositorios con Interfaz
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IDepartamentosRepository, DepartamentosRepository>();
builder.Services.AddScoped<ITicketsRepository, TicketsRepository>();
builder.Services.AddScoped<IComentariosRepository, ComentariosRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

//Registrar los servicios con interfaz
builder.Services.AddScoped<IDepartamentoService, DepartamentosService>();
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IComentariosService, ComentariosService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configurar la autenticación
builder.Services.AddAuthentication
    (
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            RoleClaimType = ClaimTypes.Role,
            ValidIssuer = issuer,
            ValidAudience = audience
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = 401,
                    detail = "No autenticado. El token es inválido o no fue enviado."
                }));
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = 403,
                    detail = "Acceso denegado. No tiene permisos para acceder a este recurso."
                }));
            }
        };
    });


// Registrar AutoMapper
builder.Services.AddAutoMapper(cgf => { }, typeof(MappingProfile).Assembly);



builder.Services.AddControllers();




//swager openAi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TicketSoporte API",
        Description = """
        #### **Infraestructura escalable para la gestión de tickets de soporte técnico.**

        Esta API proporciona un conjunto robusto de herramientas para administrar incidencias técnicas, garantizando un seguimiento eficiente y una resolución optimizada de requerimientos.

        ---

        #### Módulos del Sistema
        * **Tickets:** Gestión del ciclo de vida de incidencias y requerimientos técnicos.
        * **Departamentos:** Organización de áreas resolutoras y asignación de personal.
        * **Comentarios:** Registro cronológico de soluciones e interacciones en cada ticket.
        * **Usuarios & Auth:** Control de acceso basado en roles (Admin/Técnico) con seguridad JWT.

        #### Características Técnicas
        * **Seguridad:** Autenticación mediante **JSON Web Tokens (JWT)** y Refresh Tokens.
        * **Arquitectura:** Basada en patrones de Repositorio y Servicios para máxima escalabilidad.
        * **Documentación:** Interfaz interactiva para pruebas de endpoints en tiempo real.

        ---

        """,
        Contact = new OpenApiContact
        {
            Name = "Miguel Antonio Baldelomar (Soporte Técnico)",
            Email = "hurtadomiguel296@gmail.com", 
            Url = new Uri("https://github.com/miguelhurtad0/TicketSoporte")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Configuración de seguridad para Swagger (JWT)

    // 1. Definir el esquema de seguridad que Swagger usará para UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT. Ejemplo: eyJhbGciOiJIUzI1NiIsInR5..."
    });

    // 2. Aplicar el esquema de seguridad a toso los endpoint protegidos de la API
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(referenceId: "Bearer", hostDocument: document),
            new List<string>()
        }
    });
});

// Configuración de CORS
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins(
                "http://localhost:4200",    // Angular
                "http://localhost:3000"    // React
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        }
        else
        {
            // Solo para desarrollo si no hay configuración
            policy.AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});


builder.Services.AddOpenApi();

// Construir la aplicación
var app = builder.Build();


// Registrar el middleware de manejo de excepciones
app.UseMiddleware<ExceptionMiddleware>();




app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TicketSoporte API v1");
});


app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});


app.UseCors("FrontendPolicy");

// Soporte para la autenticación
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();



if (app.Environment.IsDevelopment())
{
    app.Run();

}
else
{
    var apiPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Run($"http://0.0.0.0:{apiPort}");


}




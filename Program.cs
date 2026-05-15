using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using iCarus.Src.Db;
using iCarus.Src.Services;
using iCarus.Src.Services.interfaces;
using iCarus.Src.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Auth
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ISendGridEmailServices, SendGridEmailServices>();

// Cloudinary
builder.Services.AddScoped<ICloudinaryServices, CloudinaryServices>();

// Funcion
builder.Services.AddScoped<IFuncionServices, FuncionServices>();

// Base de datos
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configuración de CORS, permite solicitudes desde el origen http://localhost:5173 
// lo que ayuda al frontend comunicarse a esta api
builder.Services.AddCors(options =>
{
    // Configuración de una política de CORS llamada "AllowAll" 
    // que permite solicitudes desde el origen "http://localhost:5173",
    options.AddPolicy("AllowAll", policy =>
    {
        // Configuración de la política de CORS para permitir solicitudes desde el origen "http://localhost:5173",
        // permitiendo así que una aplicación frontend (como una aplicación React) pueda comunicarse con esta API 
        // sin restricciones de origen cruzado,
        policy.WithOrigins("http://localhost:5173","https://icarus-frontend.onrender.com")
            // Verifica content-type o authorization
            .AllowAnyHeader()
            // Acepta métodos http
            .AllowAnyMethod()
            // Permite enviar cookies
            .AllowCredentials();
    });
}); 

builder.Services.AddDbContext<ContextDb>(options =>
    options.UseNpgsql(ConnectionString)
);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:issuer"],
            ValidAudience = builder.Configuration["Jwt:audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ContextDb>();
        await new Seeder(context).Seed();
        Console.WriteLine("Base de datos poblada exitosamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al poblar la base de datos: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

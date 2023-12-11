using ApiPeliculas.Data;
using ChallengeCFOTech.Mappers;
using ChallengeCFOTech.Repositories;
using ChallengeCFOTech.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CONEXIÓN BASE DE DATOS
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

// AGREGAR REPOSITORIOS
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<ICharacter, CharacterRepository>();

// SOPORTE AUTOMAPPER
builder.Services.AddAutoMapper(typeof(IndexMapper));

//CONFIGURACIÓN DE LA AUTENTICACIÓN
var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

builder.Services.AddAuthentication(schema =>
{
    schema.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    schema.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(schema =>
{
    schema.RequireHttpsMetadata = false;
    schema.SaveToken = true;
    schema.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// SOPORTE CACHE GLOBAL
builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add("PorDefecto30seg", new CacheProfile()
    {
        Duration = 30
    });
});

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// SOPORTE PARA AUTENTICACIÓN EN SWAGGER
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT uando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' seguida de un[espacio] y despues su token en el campo de abajo \r\n\r\n " +
        "Ejemplo: \"Bearer sdfs9fjs9fsf3df2df\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme= "auth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }

    });
});

// SOPORTE PARA CORS
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//SOPORTE PARA CORS
app.UseCors("PolicyCors");

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

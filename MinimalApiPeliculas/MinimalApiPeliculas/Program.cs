
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas;
using MinimalApiPeliculas.EndPoints;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;
var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;
//Inicio del area de los servicios
builder.Services.AddDbContext<ApplicationDbContext>(opciones=> 
    opciones.UseSqlServer("name=DefaultConnection"));
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryGenero, RepositoryGenero>();
builder.Services.AddScoped<IRepositoryActores, RepositoryActores>();
builder.Services.AddScoped<IAlmacenadorArchivos,AlmacenadorArchivoAzure>();
//builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivoLocal>();
builder.Services.AddScoped<IRepositoryPeliculas, RepositoryPeliculas>();
builder.Services.AddScoped<IRepositoryComentarios, RepositoryComentarios>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
//Fin del area de los servicios
var app = builder.Build();
//Inicio del area del middleware

app.UseSwagger();
app.UseSwaggerUI();
//Para guardar Archivos de manera local
//app.UseStaticFiles();
app.UseCors();
app.UseOutputCache();
app.MapGet("/", () => "Hola mundo!");
   
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MappComentarios();


//Fin del area de los middleware
app.Run();

﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.DTO;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositoryPeliculas : IRepositoryPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositoryPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }
        public async Task<Pelicula> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(p=>p.Comentarios)
                .Include(p=>p.GenerosPeliculas)
                    .ThenInclude(gp=>gp.Genero)
                .Include(p=>p.ActoresPeliculas.OrderBy(x=>x.Orden))
                    .ThenInclude(ap=>ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }
        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }
        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }

        public async Task AsignarGeneros (int id, List<int> generosIds)
        {
            var pelicula = await context.Peliculas
                .Include(p=>p.GenerosPeliculas)
                .FirstOrDefaultAsync(p=>p.Id == id);

            if(pelicula is null)
            {
                throw new ArgumentException($"No existe una pelicula con el id {id}");
            }

            var generosPeliculas=generosIds.Select(generoId => new GeneroPelicula() { GeneroId= generoId });
            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);
            await context.SaveChangesAsync();
        }
        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for(int i=1; i<=actores.Count; i++)
            {
                actores[i-1].Orden = i;
            }
            var pelicula= await context.Peliculas
                .Include(x=>x.ActoresPeliculas)
                .FirstOrDefaultAsync(x=>x.Id == id);
            if(pelicula is null)
            {
                throw new ArgumentException($"No existe la pelicula con id: {id}");
            }
            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);
            await context.SaveChangesAsync();
        }
    }
}

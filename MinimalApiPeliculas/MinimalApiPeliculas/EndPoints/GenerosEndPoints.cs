using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTO;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.EndPoints
{
    public static class GenerosEndPoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", obtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

            group.MapGet("/{id:int}", obtenerGeneroPorId);

            group.MapPost("/", crearGenero);

            group.MapPut("/{id:int}", actualizarGenero);

            group.MapDelete("/{id:int}", borrarGenero);
            return group;
        }
        static async Task<Ok<List<GeneroDTO>>> obtenerGeneros(IRepositoryGenero repositorio,IMapper mapper)
        {
            var generos = await repositorio.ObtenerGeneros();

            var generosDto = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDto);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> obtenerGeneroPorId(IRepositoryGenero repositorio, int id, IMapper mappeer)
        {
            var genero = await repositorio.ObtenerGeneroPorId(id);
            if (genero == null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO=mappeer.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Created<GeneroDTO>> crearGenero(CrearGeneroDTO CrearGeneroDTO, IRepositoryGenero repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genero = mapper.Map<Genero>(CrearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
 
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> actualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositoryGenero repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id; 
            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> borrarGenero(int id, IRepositoryGenero repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}

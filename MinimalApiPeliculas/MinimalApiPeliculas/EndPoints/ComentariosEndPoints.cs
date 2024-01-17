using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTO;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.EndPoints
{
    public static class ComentariosEndPoints
    {
        public static RouteGroupBuilder MappComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag("comentarios-get")
                .SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}",ObtenerPorId);
            group.MapPost("/", Crear);
            group.MapPut("/{id:int}",Actualizar);
            group.MapDelete("/{id:int}", Borrar);
            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>,NotFound>>ObtenerTodos(int peliculaId,
            IRepositoryComentarios repositorioComentarios, IRepositoryPeliculas repositorioPeliculas, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id, IRepositoryComentarios repositorio, IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);
            if(comentario is null)
            {
                return TypedResults.NotFound();
            }
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>,NotFound>> Crear(int peliculaId, CrearComentarioDTO crearComentarioDTO,
            IRepositoryComentarios repositorioComentarios, IRepositoryPeliculas repositorioPeliculas, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            if(!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id= await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO=mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario{id}",comentarioDTO);
        }

        static async Task<Results<NoContent,NotFound>> Actualizar(int peliculaId, int id, CrearComentarioDTO crearComentarioDTO, 
            IOutputCacheStore outputCacheStore, IRepositoryComentarios repositorioComentarios, IRepositoryPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.Id=id;
            comentario.PeliculaId=peliculaId;
            await repositorioComentarios.Actualizar(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent,NotFound>> Borrar(int peliculaId, int id, IRepositoryComentarios repositorio, IOutputCacheStore outputCacheStore)
        {
            if (!await repositorio.Existe(id))
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}

using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositoryGenero
    {
        Task<List<Genero>> ObtenerGeneros();
        Task<Genero?> ObtenerGeneroPorId(int id);
        Task<int> Crear(Genero genero);

        Task<bool> Existe(int id);
        Task Actualizar(Genero genero);

        Task Borrar(int id);
        Task<List<int>> Existen(List<int> ids);
    }
}

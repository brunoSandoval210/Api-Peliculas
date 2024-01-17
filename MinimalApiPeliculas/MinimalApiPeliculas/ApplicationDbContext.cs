using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genero>().Property(propa => propa.Nombre).HasMaxLength(50);
            modelBuilder.Entity<Actor>().Property (p=> p.Nombre).HasMaxLength (150);
            modelBuilder.Entity<Actor>().Property(p=>p.Foto).IsUnicode();
            modelBuilder.Entity<Pelicula>().Property(p=>p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p=>p.Poster).IsUnicode();

            modelBuilder.Entity<GeneroPelicula>().HasKey(g => new { g.GeneroId, g.PeliculaId });
            modelBuilder.Entity<ActorPelicula>().HasKey(g => new { g.PeliculaId, g.ActorId });
        }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set;}
        public DbSet<Comentario>Comentarios { get; set; }
        public DbSet<GeneroPelicula> GenerosPeliculas { get; set; }
        public DbSet<ActorPelicula> ActoresPeliculas { get; set; }
    }
}

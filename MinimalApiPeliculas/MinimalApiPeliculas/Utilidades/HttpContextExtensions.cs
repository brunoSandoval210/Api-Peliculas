using Microsoft.EntityFrameworkCore;

namespace MinimalApiPeliculas.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,IQueryable<T> queryable)
        {
            if(queryable is null) { throw new ArgumentNullException(nameof(httpContext)); }
            
            double cantidad= await queryable.CountAsync();
            httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
            
        }
    }
}

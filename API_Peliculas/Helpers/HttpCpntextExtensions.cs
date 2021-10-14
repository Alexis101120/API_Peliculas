using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Helpers
{
    public static class HttpCpntextExtensions
    {
        public async static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext,
            IQueryable<T> queryble, int cantidadRegistros)
        {
            double cantidad = await queryble.CountAsync();
            double CantidadPaginas = Math.Ceiling(cantidad / cantidadRegistros);
            httpContext.Response.Headers.Add("cantidadPaginas", CantidadPaginas.ToString());
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPInventarios.Data;
using RPInventarios.Models;

namespace RPInventarios.Pages.Marcas;

public class IndexModel : PageModel
{
    private readonly InventariosContext _context;
    private readonly IConfiguration _configuration;

    public IndexModel(InventariosContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public List<Marca> Marcas { get; set; } = default!;
    // Propiedades de Búsqueda
    [BindProperty(SupportsGet = true)]
    public string TerminoBusqueda { get; set; }
    public int TotalRegistros { get; set; }
    //Propiedades de Paginación
    [BindProperty(SupportsGet = true)]
    public int? Pagina { get; set; }
    public int TotalPaginas { get; set; }

    public async Task OnGetAsync()
    {
        // Búsqueda o Filtrado
        var consulta = _context.Marcas.AsNoTracking();

        if (!string.IsNullOrEmpty(TerminoBusqueda))
        {
            consulta = consulta.Where(m => m.Nombre.Contains(TerminoBusqueda));
        }

        TotalRegistros = await consulta.CountAsync();

        // Paginación
        var numeroPagina = Pagina ?? 1;
        var registrosPorPagina = _configuration.GetValue("RegistrosPorPagina", 10);
        TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / registrosPorPagina);

        Marcas = await consulta
            .OrderBy(d => d.Id)
            .Skip((numeroPagina - 1) * registrosPorPagina)
            .Take(registrosPorPagina)
            .ToListAsync();
    }
}
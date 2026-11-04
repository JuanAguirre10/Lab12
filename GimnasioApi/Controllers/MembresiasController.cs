using GimnasioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GimnasioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembresiasController : ControllerBase
{
    private readonly GimnasioContext _context;

    public MembresiasController(GimnasioContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Membresia>>> Get()
    {
        var membresias = await _context.Membresias
            .AsNoTracking()
            .OrderBy(m => m.Precio)
            .ToListAsync();
        return Ok(membresias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Membresia>> Get(int id)
    {
        var membresia = await _context.Membresias.FindAsync(id);
        if (membresia == null) return NotFound();
        return Ok(membresia);
    }

    [HttpGet("{id}/socios")]
    public async Task<ActionResult<IEnumerable<Socio>>> GetSocios(int id)
    {
        if (!await _context.Membresias.AnyAsync(m => m.Id == id)) return NotFound();

        var socios = await _context.Socios
            .AsNoTracking()
            .Where(s => s.MembresiaId == id)
            .OrderBy(s => s.Nombres)
            .ToListAsync();
        return Ok(socios);
    }

    [HttpPost]
    public async Task<ActionResult<Membresia>> Post(Membresia membresia)
    {
        if (await _context.Membresias.AnyAsync(m => m.Nombre == membresia.Nombre))
            return Conflict(new ProblemDetails { Title = "Membresía duplicada", Detail = $"Ya existe la membresía '{membresia.Nombre}'.", Status = StatusCodes.Status409Conflict });

        _context.Membresias.Add(membresia);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = membresia.Id }, membresia);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Membresia membresia)
    {
        if (id != membresia.Id) return BadRequest();

        var actual = await _context.Membresias.FindAsync(id);
        if (actual == null) return NotFound();

        if (await _context.Membresias.AnyAsync(m => m.Nombre == membresia.Nombre && m.Id != id))
            return Conflict(new ProblemDetails { Title = "Membresía duplicada", Detail = $"Ya existe la membresía '{membresia.Nombre}'.", Status = StatusCodes.Status409Conflict });

        actual.Nombre = membresia.Nombre;
        actual.Precio = membresia.Precio;
        actual.DuracionDias = membresia.DuracionDias;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var membresia = await _context.Membresias.FindAsync(id);
        if (membresia == null) return NotFound();

        if (await _context.Socios.AnyAsync(s => s.MembresiaId == id))
            return Conflict(new ProblemDetails { Title = "Membresía con socios", Detail = "No se puede eliminar una membresía que tiene socios.", Status = StatusCodes.Status409Conflict });

        _context.Membresias.Remove(membresia);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

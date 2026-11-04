using GimnasioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GimnasioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SociosController : ControllerBase
{
    private readonly GimnasioContext _context;

    public SociosController(GimnasioContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Socio>>> Get(int? membresiaId, string? buscar)
    {
        var consulta = _context.Socios.Include(s => s.Membresia).AsNoTracking();

        if (membresiaId != null)
            consulta = consulta.Where(s => s.MembresiaId == membresiaId);
        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(s => s.Nombres.Contains(buscar) || s.Dni.Contains(buscar));

        var socios = await consulta.OrderBy(s => s.Nombres).ToListAsync();
        return Ok(socios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Socio>> Get(int id)
    {
        var socio = await _context.Socios
            .Include(s => s.Membresia)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
        if (socio == null) return NotFound();
        return Ok(socio);
    }

    [HttpPost]
    public async Task<ActionResult<Socio>> Post(Socio socio)
    {
        if (!await _context.Membresias.AnyAsync(m => m.Id == socio.MembresiaId))
        {
            ModelState.AddModelError(nameof(socio.MembresiaId), "La membresía indicada no existe.");
            return ValidationProblem(ModelState);
        }

        if (await _context.Socios.AnyAsync(s => s.Dni == socio.Dni))
            return Conflict(new ProblemDetails { Title = "DNI duplicado", Detail = $"Ya existe un socio con el DNI {socio.Dni}.", Status = StatusCodes.Status409Conflict });

        socio.Membresia = null;
        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = socio.Id }, socio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Socio socio)
    {
        if (id != socio.Id) return BadRequest();

        var actual = await _context.Socios.FindAsync(id);
        if (actual == null) return NotFound();

        if (!await _context.Membresias.AnyAsync(m => m.Id == socio.MembresiaId))
        {
            ModelState.AddModelError(nameof(socio.MembresiaId), "La membresía indicada no existe.");
            return ValidationProblem(ModelState);
        }

        if (await _context.Socios.AnyAsync(s => s.Dni == socio.Dni && s.Id != id))
            return Conflict(new ProblemDetails { Title = "DNI duplicado", Detail = $"Ya existe un socio con el DNI {socio.Dni}.", Status = StatusCodes.Status409Conflict });

        actual.Nombres = socio.Nombres;
        actual.Dni = socio.Dni;
        actual.Telefono = socio.Telefono;
        actual.FechaInscripcion = socio.FechaInscripcion;
        actual.MembresiaId = socio.MembresiaId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio == null) return NotFound();

        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

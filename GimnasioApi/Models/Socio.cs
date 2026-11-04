using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GimnasioApi.Models;

public partial class Socio
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
    public string Dni { get; set; } = null!;

    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar con 9.")]
    public string? Telefono { get; set; }

    public DateOnly FechaInscripcion { get; set; }

    public int MembresiaId { get; set; }

    public virtual Membresia? Membresia { get; set; }
}

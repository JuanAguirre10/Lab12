using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GimnasioApi.Models;

public partial class Membresia
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Range(0.01, 10000, ErrorMessage = "El precio debe estar entre 0.01 y 10000.")]
    public decimal Precio { get; set; }

    [Range(1, 365, ErrorMessage = "La duración debe estar entre 1 y 365 días.")]
    public int DuracionDias { get; set; }

    [JsonIgnore]
    public virtual ICollection<Socio> Socios { get; set; } = new List<Socio>();
}

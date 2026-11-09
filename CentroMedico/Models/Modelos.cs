using System.Text.Json.Serialization;

namespace CentroMedico.Models;

// Un "modelo" es una clase que solo guarda datos. Acá están todos los de la app.

// Persona que puede ingresar a la app (paciente o administrador)
public class Usuario
{
    public string Dni { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string ClaveHash { get; set; } = "";   // la contraseña NUNCA se guarda en texto, solo su "hash"
}

// Médico del centro
public class Profesional
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public List<DayOfWeek> DiasAtencion { get; set; } = new();
    public List<string> Horarios { get; set; } = new();   // por ejemplo "08:00", "08:30"
    public bool SoloParticular { get; set; }

    // Propiedades calculadas: la pantalla las usa para mostrar texto listo
    public string DiasTexto => string.Join(", ", DiasAtencion.Select(d => Dias.Corto(d)));
    public string Cobertura => SoloParticular ? "Atiende solo de forma particular" : "Atiende con obras sociales y particular";
}

// Obra social con su porcentaje de cobertura
public class ObraSocial
{
    public string Nombre { get; set; } = "";
    public int Cobertura { get; set; }
    public string CoberturaTexto => $"{Cobertura}% cubierto";
}

// Un horario libre que se muestra como botón
public class HorarioDisponible
{
    public DateTime FechaHora { get; set; }
    public string Texto => FechaHora.ToString("HH:mm");
}

// Turno reservado
public class Turno
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DniPaciente { get; set; } = "";
    public string NombrePaciente { get; set; } = "";
    public int ProfesionalId { get; set; }
    public string NombreProfesional { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public DateTime FechaHora { get; set; }

    // [JsonIgnore] = no se guardan en el almacenamiento, se calculan al mostrarlos
    [JsonIgnore]
    public string Descripcion => $"{Especialidad} - {NombreProfesional}";

    [JsonIgnore]
    public string FechaTexto => $"{Dias.Largo(FechaHora.DayOfWeek)} {FechaHora:dd/MM/yyyy} a las {FechaHora:HH:mm} hs";
}

// Nombres de los días en español
public static class Dias
{
    private static readonly string[] Cortos = { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
    private static readonly string[] Largos = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

    public static string Corto(DayOfWeek dia) => Cortos[(int)dia];
    public static string Largo(DayOfWeek dia) => Largos[(int)dia];
}
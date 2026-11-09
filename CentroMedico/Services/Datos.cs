using CentroMedico.Models;

namespace CentroMedico.Services;


public static class Datos
{
    //   USUARIOS 
    public static readonly List<Usuario> Usuarios = new()
    {
        new Usuario { Dni = "44647737", Nombre = "Franco Escarlon", ClaveHash = Seguridad.Hash("1234") },
        new Usuario { Dni = "28456789", Nombre = "Carlos Pérez", ClaveHash = Seguridad.Hash("1234") },
    };

    //  PROFESIONALES 
    public static readonly List<Profesional> Profesionales = new()
    {
        new Profesional { Id = 1, Nombre = "Dr. Alberto Fernández", Especialidad = "Clínica médica",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, Horarios = Horas(8, 12) },
        new Profesional { Id = 2, Nombre = "Dra. Laura Benítez", Especialidad = "Clínica médica",
            DiasAtencion = new() { DayOfWeek.Tuesday, DayOfWeek.Thursday }, Horarios = Horas(14, 18) },

        new Profesional { Id = 3, Nombre = "Dr. Esteban Ríos", Especialidad = "Cardiología",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Thursday }, Horarios = Horas(9, 13) },
        new Profesional { Id = 4, Nombre = "Dra. Carolina Duarte", Especialidad = "Cardiología",
            DiasAtencion = new() { DayOfWeek.Tuesday, DayOfWeek.Friday }, Horarios = Horas(15, 19) },

        new Profesional { Id = 5, Nombre = "Dra. Silvia Villalba", Especialidad = "Pediatría",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday }, Horarios = Horas(8, 12) },
        new Profesional { Id = 6, Nombre = "Dr. Gustavo Medina", Especialidad = "Pediatría",
            DiasAtencion = new() { DayOfWeek.Thursday, DayOfWeek.Friday }, Horarios = Horas(14, 18) },

        new Profesional { Id = 7, Nombre = "Dr. Ricardo Sosa", Especialidad = "Traumatología",
            DiasAtencion = new() { DayOfWeek.Tuesday, DayOfWeek.Thursday }, Horarios = Horas(8, 12) },
        new Profesional { Id = 8, Nombre = "Dr. Pablo Aguirre", Especialidad = "Traumatología",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Wednesday }, Horarios = Horas(15, 19), SoloParticular = true },

        new Profesional { Id = 9, Nombre = "Dra. Sofía Romero", Especialidad = "Dermatología",
            DiasAtencion = new() { DayOfWeek.Wednesday, DayOfWeek.Friday }, Horarios = Horas(9, 13), SoloParticular = true },
        new Profesional { Id = 10, Nombre = "Dr. Julián Cabrera", Especialidad = "Dermatología",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Tuesday }, Horarios = Horas(15, 18) },

        new Profesional { Id = 11, Nombre = "Dr. Martín Acosta", Especialidad = "Oftalmología",
            DiasAtencion = new() { DayOfWeek.Tuesday, DayOfWeek.Thursday }, Horarios = Horas(9, 12), SoloParticular = true },
        new Profesional { Id = 12, Nombre = "Dra. Natalia Giménez", Especialidad = "Oftalmología",
            DiasAtencion = new() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, Horarios = Horas(14, 17) },
    };

    // Lista de especialidades (sin repetir y ordenadas), sacada de los profesionales
    public static List<string> Especialidades =>
        Profesionales.Select(p => p.Especialidad).Distinct().OrderBy(e => e).ToList();

    // ---------- OBRAS SOCIALES ----------
    // Los porcentajes son de ejemplo (inventados para la demo)
    public static readonly List<ObraSocial> ObrasSociales = new()
    {
        new ObraSocial { Nombre = "PAMI", Cobertura = 100 },
        new ObraSocial { Nombre = "OSDE", Cobertura = 80 },
        new ObraSocial { Nombre = "Swiss Medical", Cobertura = 70 },
        new ObraSocial { Nombre = "Galeno", Cobertura = 70 },
        new ObraSocial { Nombre = "Medifé", Cobertura = 70 },
        new ObraSocial { Nombre = "Sancor Salud", Cobertura = 60 },
        new ObraSocial { Nombre = "OSECAC", Cobertura = 80 },
        new ObraSocial { Nombre = "Unión Personal", Cobertura = 80 },
        new ObraSocial { Nombre = "Jerárquicos Salud", Cobertura = 70 },
        new ObraSocial { Nombre = "OMINT", Cobertura = 60 },
    };

    // Genera horarios cada media hora. Horas(8, 12) = 08:00, 08:30, ... 11:30
    private static List<string> Horas(int desde, int hasta)
    {
        var lista = new List<string>();
        for (int hora = desde; hora < hasta; hora++)
        {
            lista.Add($"{hora:00}:00");
            lista.Add($"{hora:00}:30");
        }
        return lista;
    }
}
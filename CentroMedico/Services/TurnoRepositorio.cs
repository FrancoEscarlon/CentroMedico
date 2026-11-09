using System.Text.Json;
using CentroMedico.Models;
using Microsoft.Maui.Storage;

namespace CentroMedico.Services;

// Se encarga de guardar, buscar y cancelar turnos.
// Los turnos quedan guardados en el teléfono (Preferences), así que no se pierden al cerrar la app.
public static class TurnoRepositorio
{
    private const string Clave = "turnos_v1";
    private static List<Turno>? _turnos;

    private static List<Turno> Cargar()
    {
        if (_turnos != null) return _turnos;

        try
        {
            string json = Preferences.Default.Get(Clave, "");
            _turnos = string.IsNullOrEmpty(json)
                ? new List<Turno>()
                : JsonSerializer.Deserialize<List<Turno>>(json) ?? new List<Turno>();
        }
        catch
        {
            _turnos = new List<Turno>();
        }
        return _turnos;
    }

    private static void Guardar()
    {
        try
        {
            string json = JsonSerializer.Serialize(Cargar());
            Preferences.Default.Set(Clave, json);
        }
        catch
        {
            // Si no se pudo guardar, la demo sigue funcionando con los datos en memoria
        }
    }

    // ---------- CONSULTAS ----------

    public static List<Turno> Todos()
    {
        return Cargar().OrderBy(t => t.FechaHora).ToList();
    }

    public static List<Turno> DePaciente(string dni)
    {
        return Cargar().Where(t => t.DniPaciente == dni).OrderBy(t => t.FechaHora).ToList();
    }

    public static bool EstaOcupado(int profesionalId, DateTime fechaHora)
    {
        return Cargar().Any(t => t.ProfesionalId == profesionalId && t.FechaHora == fechaHora);
    }

    // Horarios libres de un profesional en un día (ocupados y pasados quedan afuera)
    public static List<HorarioDisponible> HorariosLibres(Profesional profesional, DateTime dia)
    {
        var libres = new List<HorarioDisponible>();

        foreach (string hora in profesional.Horarios)
        {
            DateTime fechaHora = dia.Date + TimeSpan.Parse(hora);

            bool yaPaso = fechaHora <= DateTime.Now;
            if (!yaPaso && !EstaOcupado(profesional.Id, fechaHora))
            {
                libres.Add(new HorarioDisponible { FechaHora = fechaHora });
            }
        }
        return libres;
    }

    // Próximos 14 días en los que el profesional atiende y todavía tiene lugar
    public static List<DateTime> FechasConAtencion(Profesional profesional)
    {
        var fechas = new List<DateTime>();

        for (int i = 0; i < 14; i++)
        {
            DateTime dia = DateTime.Today.AddDays(i);

            if (profesional.DiasAtencion.Contains(dia.DayOfWeek) && HorariosLibres(profesional, dia).Count > 0)
            {
                fechas.Add(dia);
            }
        }
        return fechas;
    }

    // ---------- ACCIONES ----------

    public static void Agregar(Turno turno)
    {
        Cargar().Add(turno);
        Guardar();
    }

    public static void Cancelar(Guid id)
    {
        Cargar().RemoveAll(t => t.Id == id);
        Guardar();
    }
}
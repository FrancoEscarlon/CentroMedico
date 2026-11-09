using CentroMedico.Models;
using CentroMedico.Services;

namespace CentroMedico.Pages;

public partial class PrincipalPage : ContentPage
{
    private Profesional? _profesionalElegido;
    private List<DateTime> _fechas = new();
    private string _dniMostrado = "";

    public PrincipalPage()
    {
        InitializeComponent();
    }

    // Se ejecuta cada vez que aparece la pantalla
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Seguridad: sin sesión iniciada no se puede estar acá
        if (!Sesion.EstaLogueado)
        {
            _ = Shell.Current.GoToAsync("//Login");
            return;
        }

        Usuario usuario = Sesion.Actual!;

        // Si entró otra persona, se empieza de cero
        if (_dniMostrado != usuario.Dni)
        {
            _dniMostrado = usuario.Dni;
            ReiniciarPasos();
        }

        LblSaludo.Text = $"Hola, {usuario.Nombre}";

        LblRol.Text = "Ingresaste como paciente.";
        BtnTurnos.Text = "Mis turnos";

        MostrarEspecialidades(BuscadorEspecialidad.Text ?? "");

        // Si volvió desde "Mis turnos" (por ejemplo después de cancelar uno), se actualizan los horarios
        if (_profesionalElegido != null)
        {
            CargarFechas();
        }
    }

    private void ReiniciarPasos()
    {
        _profesionalElegido = null;
        BuscadorEspecialidad.Text = "";
        SeccionProfesionales.IsVisible = false;
        SeccionHorarios.IsVisible = false;
    }

    // ---------- PASO 1: ESPECIALIDADES ----------

    private void MostrarEspecialidades(string texto)
    {
        string filtro = texto.Trim().ToLower();

        // Una especialidad se muestra si su nombre coincide con lo escrito
        // o si algún médico de esa especialidad coincide
        List<string> lista = Datos.Especialidades
            .Where(esp => filtro == ""
                || esp.ToLower().Contains(filtro)
                || Datos.Profesionales.Any(p => p.Especialidad == esp && p.Nombre.ToLower().Contains(filtro)))
            .ToList();

        BindableLayout.SetItemsSource(ListaEspecialidades, lista);
        LblSinResultados.IsVisible = lista.Count == 0;
    }

    private void AlCambiarBusqueda(object sender, TextChangedEventArgs e)
    {
        MostrarEspecialidades(e.NewTextValue ?? "");
    }

    private async void AlElegirEspecialidad(object sender, EventArgs e)
    {
        string especialidad = ((Button)sender).Text;

        List<Profesional> profesionales = Datos.Profesionales
            .Where(p => p.Especialidad == especialidad)
            .ToList();

        _profesionalElegido = null;
        LblTituloProfesionales.Text = $"2. Médicos de {especialidad}";
        BindableLayout.SetItemsSource(ListaProfesionales, profesionales);

        SeccionProfesionales.IsVisible = true;
        SeccionHorarios.IsVisible = false;

        await DesplazarHasta(SeccionProfesionales);
    }

    // ---------- PASO 2: PROFESIONALES ----------

    private async void AlElegirProfesional(object sender, EventArgs e)
    {
        // El botón pertenece a una tarjeta; su BindingContext es el médico de esa tarjeta
        _profesionalElegido = (Profesional)((Button)sender).BindingContext;

        LblTituloHorarios.Text = $"3. Horarios de {_profesionalElegido.Nombre}";
        CargarFechas();

        SeccionHorarios.IsVisible = true;

        await DesplazarHasta(SeccionHorarios);
    }

    // ---------- PASO 3: HORARIOS ----------

    private void CargarFechas()
    {
        if (_profesionalElegido == null) return;

        _fechas = TurnoRepositorio.FechasConAtencion(_profesionalElegido);
        PickerDia.ItemsSource = _fechas
            .Select(f => $"{Dias.Largo(f.DayOfWeek)} {f:dd/MM}")
            .ToList();

        if (_fechas.Count == 0)
        {
            BindableLayout.SetItemsSource(ListaHorarios, new List<HorarioDisponible>());
            LblHorarios.Text = "Este médico no tiene horarios disponibles en los próximos 14 días.";
            return;
        }

        PickerDia.SelectedIndex = 0;
        MostrarHorarios();
    }

    private void AlCambiarDia(object sender, EventArgs e)
    {
        MostrarHorarios();
    }

    private void MostrarHorarios()
    {
        int posicion = PickerDia.SelectedIndex;
        if (_profesionalElegido == null || posicion < 0 || posicion >= _fechas.Count) return;

        List<HorarioDisponible> libres = TurnoRepositorio.HorariosLibres(_profesionalElegido, _fechas[posicion]);
        BindableLayout.SetItemsSource(ListaHorarios, libres);

        LblHorarios.Text = libres.Count > 0
            ? "Tocá un horario para reservar tu turno:"
            : "No quedan horarios libres ese día. Probá con otro día.";
    }

    // Se ejecuta al tocar un horario: reserva el turno
    private async void AlElegirHorario(object sender, EventArgs e)
    {
        if (_profesionalElegido == null) return;

        HorarioDisponible horario = (HorarioDisponible)((Button)sender).BindingContext;

        string dniPaciente = Sesion.Actual!.Dni;
        string nombrePaciente = Sesion.Actual.Nombre;

        string cuando = $"{Dias.Largo(horario.FechaHora.DayOfWeek)} {horario.FechaHora:dd/MM/yyyy} a las {horario.Texto} hs";

        bool confirmar = await DisplayAlert(
            "Confirmar turno",
            $"¿Reservar turno para {nombrePaciente} con {_profesionalElegido.Nombre} ({_profesionalElegido.Especialidad}) el {cuando}?",
            "Confirmar", "Cancelar");

        if (!confirmar) return;

        // Se vuelve a controlar por si el horario se ocupó mientras se decidía
        if (TurnoRepositorio.EstaOcupado(_profesionalElegido.Id, horario.FechaHora))
        {
            await DisplayAlert("Horario ocupado", "Ese horario ya fue reservado. Elegí otro.", "Entendido");
            MostrarHorarios();
            return;
        }

        TurnoRepositorio.Agregar(new Turno
        {
            DniPaciente = dniPaciente,
            NombrePaciente = nombrePaciente,
            ProfesionalId = _profesionalElegido.Id,
            NombreProfesional = _profesionalElegido.Nombre,
            Especialidad = _profesionalElegido.Especialidad,
            FechaHora = horario.FechaHora
        });

        await DisplayAlert("¡Turno reservado!", $"Turno con {_profesionalElegido.Nombre}, {cuando}.", "Aceptar");

        MostrarHorarios();   // el horario reservado desaparece de la lista
    }

    // ---------- NAVEGACIÓN ----------

    private async void AlIrAInstitucional(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Institucional");
    }

    private async void AlIrAObrasSociales(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ObrasSociales");
    }

    private async void AlIrATurnos(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Turnos");
    }

    private async void AlCerrarSesion(object sender, EventArgs e)
    {
        Sesion.Cerrar();
        await Shell.Current.GoToAsync("//Login");
    }

    // Baja la pantalla hasta una sección (con una pequeña espera para que ya esté dibujada)
    private async Task DesplazarHasta(VisualElement destino)
    {
        await Task.Delay(150);
        await Scroll.ScrollToAsync(destino, ScrollToPosition.Start, true);
    }
}
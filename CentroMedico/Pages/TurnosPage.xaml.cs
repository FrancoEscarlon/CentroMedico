using CentroMedico.Models;
using CentroMedico.Services;

namespace CentroMedico.Pages;

public partial class TurnosPage : ContentPage
{
    public TurnosPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarTurnos();
    }

    private void CargarTurnos()
    {
        if (!Sesion.EstaLogueado) return;

        Title = "Mis turnos";
        LblTitulo.Text = "Mis turnos reservados";
        List<Turno> turnos = TurnoRepositorio.DePaciente(Sesion.Actual!.Dni);

        BindableLayout.SetItemsSource(ListaTurnos, turnos);
        LblVacio.IsVisible = turnos.Count == 0;
    }

    private async void AlCancelar(object sender, EventArgs e)
    {
        Turno turno = (Turno)((Button)sender).BindingContext;

        bool confirmar = await DisplayAlert(
            "Cancelar turno",
            $"¿Querés cancelar el turno con {turno.NombreProfesional} ({turno.FechaTexto})?",
            "Sí, cancelar", "No");

        if (!confirmar) return;

        TurnoRepositorio.Cancelar(turno.Id);
        CargarTurnos();
    }
}
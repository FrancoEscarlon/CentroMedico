using CentroMedico.Pages;

namespace CentroMedico;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pantallas a las que se "entra" desde otra (aparece la flecha de volver arriba).
        // El texto es el nombre que se usa en Shell.Current.GoToAsync("...")
        Routing.RegisterRoute("Institucional", typeof(InstitucionalPage));
        Routing.RegisterRoute("ObrasSociales", typeof(ObrasSocialesPage));
        Routing.RegisterRoute("Turnos", typeof(TurnosPage));
    }
}
using CentroMedico.Services;

namespace CentroMedico.Pages;

public partial class ObrasSocialesPage : ContentPage
{
    public ObrasSocialesPage()
    {
        InitializeComponent();

        // Se cargan las listas desde los datos de la app
        BindableLayout.SetItemsSource(ListaObras, Datos.ObrasSociales);
        BindableLayout.SetItemsSource(ListaParticulares, Datos.Profesionales.Where(p => p.SoloParticular).ToList());
    }
}
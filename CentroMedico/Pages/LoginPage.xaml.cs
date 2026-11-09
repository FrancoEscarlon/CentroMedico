using CentroMedico.Models;
using CentroMedico.Services;

namespace CentroMedico.Pages;

public partial class LoginPage : ContentPage
{
    private int _intentosFallidos = 0;
    private DateTime _bloqueadoHasta = DateTime.MinValue;

    public LoginPage()
    {
        InitializeComponent();
    }

    // Se ejecuta cada vez que aparece esta pantalla (al abrir la app y al cerrar sesión)
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Sesion.Cerrar();            // al volver al login, no queda ninguna sesión abierta
        EntryDni.Text = "";
        EntryClave.Text = "";
        LblError.IsVisible = false;
    }

    // Se ejecuta al tocar "Ingresar" 
    private async void AlIngresar(object sender, EventArgs e)
    {
        // 1. Si está bloqueado por demasiados intentos, no sigue
        if (DateTime.Now < _bloqueadoHasta)
        {
            int segundos = (int)(_bloqueadoHasta - DateTime.Now).TotalSeconds + 1;
            MostrarError($"Demasiados intentos. Esperá {segundos} segundos.");
            return;
        }

        string dni = (EntryDni.Text ?? "").Trim();
        string clave = EntryClave.Text ?? "";

        // 2. Validar que los datos tengan formato correcto
        if (dni.Length < 7 || !dni.All(char.IsDigit))
        {
            MostrarError("Ingresá un DNI válido: 7 u 8 números, sin puntos.");
            return;
        }
        if (clave.Length == 0)
        {
            MostrarError("Ingresá tu contraseña.");
            return;
        }

        // 3. Buscar el usuario: DNI y contraseña (comparando hashes)
        string claveHash = Seguridad.Hash(clave);

        Usuario? usuario = Datos.Usuarios.FirstOrDefault(u =>
            u.Dni == dni && u.ClaveHash == claveHash);

        // 4. Si no existe, error. El mensaje no dice si falló el DNI o la contraseña (por seguridad)
        if (usuario == null)
        {
            _intentosFallidos++;

            if (_intentosFallidos >= 3)
            {
                _intentosFallidos = 0;
                _bloqueadoHasta = DateTime.Now.AddSeconds(30);
                MostrarError("Datos incorrectos. Por seguridad, esperá 30 segundos para volver a intentar.");
            }
            else
            {
                MostrarError("DNI o contraseña incorrectos.");
            }
            return;
        }

        // 5. Todo bien: se guarda la sesión y se pasa a la pantalla principal
        _intentosFallidos = 0;
        Sesion.Iniciar(usuario);
        await Shell.Current.GoToAsync("//Principal");
    }

    private void MostrarError(string mensaje)
    {
        LblError.Text = "⚠ " + mensaje;
        LblError.IsVisible = true;
    }
}
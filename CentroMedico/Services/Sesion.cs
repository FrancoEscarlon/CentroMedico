using System.Security.Cryptography;
using System.Text;
using CentroMedico.Models;

namespace CentroMedico.Services;

// Convierte una contraseña en un código (hash) imposible de revertir.
// Así, en el programa nunca queda guardada la contraseña real.
public static class Seguridad
{
    public static string Hash(string clave)
    {
        byte[] datos = Encoding.UTF8.GetBytes(clave);
        byte[] resultado = SHA256.HashData(datos);
        return Convert.ToHexString(resultado);
    }
}

// Guarda quién está usando la app en este momento.
// "static" significa que hay una sola sesión y todas las pantallas la comparten.
public static class Sesion
{
    public static Usuario? Actual { get; private set; }

    public static bool EstaLogueado => Actual != null;

    public static void Iniciar(Usuario usuario)
    {
        Actual = usuario;
    }

    public static void Cerrar()
    {
        Actual = null;
    }
}
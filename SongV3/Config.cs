using MySql.Data.MySqlClient;
using System;

namespace SongV3
{
    public static class conexion
    {
        private static string GetConnectionString()
        {
            // Ajuste: usar un valor de SslMode compatible con el proveedor.
            // Si tu proveedor no soporta SslMode=None, usa "Preferred" o simplemente elimina la opción.
            string pwd = Environment.GetEnvironmentVariable("SONGDB_PWD") ?? "";
            return $"Server=127.0.0.1;Port=3306;Database=songguesser;Uid=root;Pwd={pwd};SslMode=Preferred;AllowPublicKeyRetrieval=True;DefaultCommandTimeout=120;";
        }

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(GetConnectionString());
        }

        // Método de diagnóstico temporal: devuelve "OK" o el mensaje de excepción completo.
        public static string TestConexion()
        {
            try
            {
                using var cn = ObtenerConexion();
                cn.Open();
                cn.Close();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
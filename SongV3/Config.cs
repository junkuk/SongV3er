using MySql.Data.MySqlClient;
using System;

namespace SongV3
{
    public static class conexion
    {
        private static string GetConnectionString()
        {
            string pwd = Environment.GetEnvironmentVariable("SONGDB_PWD") ?? "";
            return $"Server=127.0.0.1;Port=3306;Database=asongv3;Uid=root;Pwd={pwd};Pooling=true;Min Pool Size=1;Max Pool Size=100;AllowPublicKeyRetrieval=True;DefaultCommandTimeout=10;";
        }

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(GetConnectionString());
        }

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
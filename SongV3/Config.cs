using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongV3
{
    public static class conexion
    {
        public static MySqlConnection ObtenerConexion()
        {
            string connectionString = "Server=localhost;"
                                 + "Database=songguesser;"
                                 + "Uid=root;" //Este usuario se cambia al momento de usarse en la practica (dentro de la BDD)
                                 + "Pwd=;"
                                 + "Port=3306;"
                                 + "SslMode=Preferred;"
                                 + "DefaultCommandTimeout=120;";
            return new MySqlConnection(connectionString);
        }
    }
}

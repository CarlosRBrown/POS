using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Connection
    {
		public string cadena_conexion()
		{
			// Definir la cadena de conexión
			// Ajusta los valores de Server, Database, User Id y Password según tu configuración
			string connectionString = "Server=.\\SQLEXPRESS; Database=POS; User Id=sa; Password=2025;";
			return connectionString;


			// Si usas Autenticación de Windows:
			// private string connectionString = "Server=.\\SQLEXPRESS; Database=MiBaseDeDatos; Integrated Security=True;";
		}
	}
}

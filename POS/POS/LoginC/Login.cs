using Data;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LoginC
{
    public partial class Login : Form
    {
        // Ajusta los valores de Server, Database, User Id y Password según tu configuración
        private string connectionString;
		// Si usas Autenticación de Windows:
		// private string connectionString = "Server=.\\SQLEXPRESS; Database=MiBaseDeDatos; Integrated Security=True;";

		public Login()
        {
            InitializeComponent(); // Método generado automáticamente por Visual Studio
            this.ActiveControl = null;
        }

		[DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.DLL", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private void btnLogin_Click(object sender, EventArgs e)
        {
            //string username = txtUser.Text;
            //string password = txtPassword.Text; // En un entorno real, esto sería el hash de la contraseña

            //if (AuthenticateUser(username, password))
            //{
            //    MessageBox.Show("¡Login exitoso!");
            //    // Redirigir al formulario principal o área de la aplicación

            //    Dashboard F2 = new Dashboard();
            //    F2.Show();
            //    this.Hide();
            //    // Mostrar el siguiente formulario
            //}
            //else
            //{
            //    MessageBox.Show("Usuario o contraseña incorrectos.", "ERROR");


            //}













            string inputUsername = txtUser.Text; // Asume un TextBox llamado txtUsername
            string inputPassword = txtPassword.Text; //

            string query = "SELECT Id_User, Name_User, Last_Name_User FROM Users WHERE Name_User = @Username AND Password = @Password";

            int userId = -1;
            string username = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // **Uso de parámetros para seguridad (evitar inyección SQL)**
                        command.Parameters.AddWithValue("@Username", inputUsername);
                        // **Nota:** En un entorno real, la contraseña debe estar hasheada.
                        command.Parameters.AddWithValue("@Password", inputPassword);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1. Obtener el UserID (entero)
                                userId = reader.GetInt32(reader.GetOrdinal("Id_User"));

                                // 2. Obtener el Username (string)
                                username = reader.GetString(reader.GetOrdinal("Name_User"));

                                MessageBox.Show("¡Inicio de sesión exitoso!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // **Llamada y paso de datos al nuevo formulario**
                                Dashboard F2 = new Dashboard(userId, username);
                                F2.Show();
                                this.Hide(); // Ocultar el formulario de login
                            }
                            else
                            {
                                MessageBox.Show("Credenciales incorrectas. Inténtalo de nuevo.", "Error de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión o consulta: " + ex.Message, "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }





        }

        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            // La consulta SQL debe ser parametrizada para prevenir inyecciones SQL
            string query = "SELECT COUNT(1) FROM Users WHERE Name_User = @User AND Password = @Pwd";

            try
            {
				
				using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Añadir parámetros para evitar inyección SQL
                        command.Parameters.AddWithValue("@User", username);
                        command.Parameters.AddWithValue("@Pwd", password);

                        connection.Open();

                        // ExecuteScalar() es eficiente para obtener un único valor (como un COUNT)
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            isAuthenticated = true; // Se encontró un registro coincidente
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión a la base de datos: " + ex.Message);
            }

            return isAuthenticated;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            if (txtUser.Text == "USUARIO")
            {
                txtUser.Text = "";
                txtUser.ForeColor = Color.White;
            }
        }

        private void txtUser_Validated(object sender, EventArgs e)
        {

		}

        private void txtPassword_Enter(object sender, EventArgs e)
        {
			if (txtPassword.Text == "CONTRASEÑA")
			{
				txtPassword.Text = "";
				txtPassword.ForeColor = Color.White;
				txtPassword.PasswordChar = '*'; // ¡Activa los asteriscos!
			}
		}

        private void txtPassword_Leave(object sender, EventArgs e)
        {
			if (txtPassword.Text == "")
			{
				txtPassword.Text = "CONTRASEÑA";
				txtPassword.ForeColor = Color.White;
				txtPassword.PasswordChar = '\0'; // Desactiva los asteriscos
			}
		}

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}

        private void txtUser_Leave(object sender, EventArgs e)
        {
			if (txtUser.Text == "")
			{
				txtUser.Text = "USUARIO";
				txtUser.ForeColor = Color.White;
			}
		}

        private void Login_Load(object sender, EventArgs e)
        {
            this.ActiveControl = null;

			// 1. Instanciar la clase Connection de la librería Data
			Connection conexionDB = new Connection();

			// 2. Llamar al método público para obtener la cadena de conexión
			connectionString = conexionDB.cadena_conexion();

		}

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(0, 30, 80);
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(0, 82, 138);
        }
    }
}

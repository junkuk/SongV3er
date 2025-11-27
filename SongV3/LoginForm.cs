using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SongV3
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnIniciarSesion;
        private Button btnRegistrar;
        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblContrasena;
        public LoginForm()
        {
            InitializeComponent();
            Task.Run(() => {
                try
                {
                    conexion.TestConexion(); // Llama a tu método Test
                }
                catch { }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (usuario == "" || contrasena == "")
            {
                MessageBox.Show("Debe llenar todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var cn = conexion.ObtenerConexion();
                cn.Open();

                string sp_nombre = "sp_ValidarUsuario";
                MySqlCommand cmd = new MySqlCommand(sp_nombre, cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_nombre", usuario);
                cmd.Parameters.AddWithValue("@p_contrasena", contrasena);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        MessageBox.Show("Inicio de sesión correcto", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        MenuForm menu = new MenuForm(usuario);
                        menu.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
        }
    }
}


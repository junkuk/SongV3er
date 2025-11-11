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
      
            this.Size = new System.Drawing.Size(350, 310);
            this.Text = "Song Guesser - Iniciar Sesión";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245); // Fondo gris claro suave
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "Song Guesser",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 150) / 2, 20), // Centrado
                ForeColor = Color.FromArgb(0, 122, 204) // Azul moderno
            };
            Controls.Add(lblTitulo);

            // Etiqueta Usuario
            lblUsuario = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(50, 70),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            Controls.Add(lblUsuario);

            // TextBox Usuario
            txtUsuario = new TextBox
            {
                Name = "txtUsuario",
                Location = new Point(50, 90),
                Size = new System.Drawing.Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(txtUsuario);

            // Etiqueta Contraseña
            lblContrasena = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(50, 120),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            Controls.Add(lblContrasena);

            // TextBox Contraseña
            txtContrasena = new TextBox
            {
                Name = "txtContrasena",
                Location = new Point(50, 140),
                Size = new System.Drawing.Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            Controls.Add(txtContrasena);

            // Botón Iniciar Sesión
            btnIniciarSesion = new Button
            {
                Name = "btnIniciarSesion",
                Text = "Iniciar Sesión",
                Location = new Point(50, 180),
                Size = new System.Drawing.Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204), // Azul
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Boton Registro
            btnRegistrar = new Button
            {
                Name = "btnRegistrar",
                Text = "Registrarse",
                Location = new Point(50, 220),
                Size = new System.Drawing.Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204), // Azul
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnIniciarSesion.FlatAppearance.BorderSize = 0;
            btnIniciarSesion.Click += btnIniciarSesion_Click;
            Controls.Add(btnIniciarSesion);

            btnRegistrar.FlatAppearance.BorderSize = 0;
            btnRegistrar.Click += (s, e) =>
            {
                this.Hide();
                RegistroForm registroForm = new RegistroForm();
                registroForm.ShowDialog();
                this.Show();
            };
            Controls.Add(btnRegistrar);
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
                // Usamos 'using' para asegurar que la conexión se cierre
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    // 1. Nombre del Stored Procedure
                    string sp_nombre = "sp_ValidarUsuario";
                    MySqlCommand cmd = new MySqlCommand(sp_nombre, cn);

                    // 2. Especificar que el comando es un Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 3. Añadir los parámetros (deben coincidir con los del SP)
                    cmd.Parameters.AddWithValue("@p_nombre", usuario);
                    cmd.Parameters.AddWithValue("@p_contrasena", contrasena);

                    // 4. Ejecutar el comando
                    // Usamos ExecuteReader() porque esperamos que el SP devuelva filas
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            MessageBox.Show("Inicio de sesión correcto", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            MenuForm menu = new MenuForm(); // Asumiendo que existe una clase MenuForm
                            menu.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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


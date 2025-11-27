using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SongV3
{
    public partial class RegistroForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private TextBox txtEmail;
        private Button btnRegistrarUsuario;
        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblContrasena;
        private Label lblEmail;

        public RegistroForm()
        {
            InitializeComponent();

            this.Size = new System.Drawing.Size(350, 350);
            this.Text = "Song Guesser - Registro";
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

            // Etiqueta email
            lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point (50,170),
                ForeColor = Color.FromArgb (50, 50, 50)
            };
            Controls.Add(lblEmail);

            // TextBox email
            txtEmail = new TextBox
            {
                Name = "txtEmail",
                Location = new Point(50, 190),
                Size = new System.Drawing.Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
            };
            Controls.Add(txtEmail);

            // Botón Registrar
            btnRegistrarUsuario = new Button
            {
                Name = "btnRegistrarUsuario",
                Text = "Registrar Usuario",
                Location = new Point(50, 250),
                Size = new System.Drawing.Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204), // Azul
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegistrarUsuario.FlatAppearance.BorderSize = 0;
            btnRegistrarUsuario.Click += btnRegistrarUsuario_Click;
            Controls.Add(btnRegistrarUsuario);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void btnRegistrarUsuario_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();
            string email = txtEmail.Text.Trim();
            if(usuario.Contains(' '))
            {
                MessageBox.Show("No se permiten espacios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (usuario == ""|| contrasena == "" || email == "")
            {
                MessageBox.Show("Debe llenar todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (usuario.Length > 12)
                {
                    MessageBox.Show("No se permite nombre de usuario mayor a 12 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, patronEmail))
            {
                MessageBox.Show("Debe ingresar un correo electrónico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    // Nombre del Stored Procedure
                    string sp_nombre = "sp_RegistrarUsuario";
                    MySqlCommand cmd = new MySqlCommand(sp_nombre, cn);

                    // Especificar que el comando es un Stored Procedure (¡MUY IMPORTANTE!)
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_usuario", usuario);
                    cmd.Parameters.AddWithValue("@p_contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@p_email", email);

                    int filas_afectadas = cmd.ExecuteNonQuery();

                    if (filas_afectadas > 0)
                    {
                        MessageBox.Show("Usuario registrado con éxito.", "Registro Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        LoginForm menu = new LoginForm();
                        menu.ShowDialog();
                        this.Close();
                        this.Close();
                    }
                }
            }
            catch (MySqlException myEx)
            {
                if (myEx.Message.Contains("El nombre de usuario ya existe"))
                {
                    MessageBox.Show(myEx.Message, "Error de Registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error de base de datos: " + myEx.Message, "Error MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

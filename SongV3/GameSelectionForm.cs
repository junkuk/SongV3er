using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SongV3
{
    public partial class GameSelectionForm : Form
    {
        private Label lblTitulo;
        private Label lblInstruccion;
        private Button btnModo1;
        private Button btnModo2;
        private Button btnModo3;
        private Button btnVolver;
        private object username;

        private void btnModo1_Click(object sender, EventArgs e) => IniciarJuego(1);
        private void btnModo2_Click(object sender, EventArgs e) => IniciarJuego(2);
        private void btnModo3_Click(object sender, EventArgs e) => IniciarJuego(3);

        private readonly object userId;

        public GameSelectionForm(object userId)
        {
            this.userId = userId;
            InitializeComponent();

            this.Size = new System.Drawing.Size(900, 800);
            this.Text = "Song Guesser - Selección de Modo";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "Selecciona un Modo de Juego",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 400) / 2, 50),
                ForeColor = Color.FromArgb(0, 122, 204),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblTitulo);

            lblInstruccion = new Label
            {
                Text = "Elige cómo quieres desafiar tu oído musical",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 300) / 2, 100),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            Controls.Add(lblInstruccion);

            int centerX = (this.ClientSize.Width - 500) / 2;
            int startY = 160;
            int spacing = 25;

            // Modo 1 - Sigue la Letra (tu idea)
            btnModo1 = CreateModeButton(
                "Sigue la Letra",
                "Se reproducen 6 segundos de la canción...\n¡Elige cuál es el siguiente verso!",
                centerX, startY,
                Color.FromArgb(46, 204, 113) // Verde esmeralda


            );
          

            // Modo 2 - Clásico
            btnModo2 = CreateModeButton(
                "Adivina Título y Artista",
                "Escucha el fragmento y escribe el título y artista.\n¡Clásico y adictivo!",
                centerX, startY + 140 + spacing,
                Color.FromArgb(52, 152, 219) // Azul cielo
            );
           

            // Modo 3 - Solo Artista
            btnModo3 = CreateModeButton(
                "¿Quién Canta?",
                "4 opciones de artistas.\n¡Ideal para principiantes o partidas rápidas!",
                centerX, startY + (140 + spacing) * 2,
                Color.FromArgb(155, 89, 182) // Morado
            );


            // Botón Volver
            btnVolver = new Button
            {
                Text = "Volver al Menú",
                Location = new Point(this.ClientSize.Width - 180, this.ClientSize.Height - 90),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();
            Controls.Add(btnVolver);

            Controls.Add(btnModo1);
            Controls.Add(btnModo2);
            Controls.Add(btnModo3);

        }

        private Button CreateModeButton(string titulo, string descripcion, int x, int y, Color colorFondo)
        {
            Panel panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(500, 130),
                BackColor = colorFondo,
                Cursor = Cursors.Hand
            };
            panel.MouseEnter += (s, e) => panel.BackColor = ControlPaint.Light(colorFondo, 0.2f);
            panel.MouseLeave += (s, e) => panel.BackColor = colorFondo;

            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label lblDesc = new Label
            {
                Text = descripcion,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                MaximumSize = new Size(460, 0),
                Location = new Point(20, 55)
            };

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblDesc);
            panel.Click += (s, e) => IniciarJuego(titulo == "Sigue la Letra" ? 1 : titulo == "Adivina Título y Artista" ? 2 : 3);

            Controls.Add(panel);
            return new Button { Tag = panel }; // truco para que funcione el hover
        }
        private void IniciarJuego(int modo)
        {
            int newGameId = 0;
            int idUsuario = 0;

            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    // 1. Obtenemos el Id_User real a partir del username
                    // cambiar por SP
                    string sqlUser = "SELECT Id_User FROM users WHERE Username = @username";
                    using (MySqlCommand cmdUser = new MySqlCommand(sqlUser, cn))
                    {
                        cmdUser.Parameters.AddWithValue("@username", idUsuario); // this.username es string
                        object res = cmdUser.ExecuteScalar();

                        if (res == null || res == DBNull.Value)
                        {
                            MessageBox.Show("Usuario no encontrado en la base de datos.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        idUsuario = Convert.ToInt32(res);
                    }

                    // 2. Creamos la partida con el Id_User correcto (int)
                    using (MySqlCommand cmd = new MySqlCommand("sp_IniciarJuego", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_IdUser", idUsuario); // ← Ahora sí es int válido

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            newGameId = Convert.ToInt32(result);
                    }
                }

                if (newGameId > 0)
                {
                    this.Hide();
                    GameForm game = new GameForm(modo, newGameId, idUsuario);
                    game.ShowDialog();
                    this.Close(); // o this.Hide() si quieres volver al menú
                }
                else
                {
                    MessageBox.Show("No se pudo crear la partida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

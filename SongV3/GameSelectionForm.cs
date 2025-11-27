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
        private string _usernameActual;

        private void btnModo1_Click(object sender, EventArgs e) => IniciarJuego(1);
        private void btnModo2_Click(object sender, EventArgs e) => IniciarJuego(2);
        private void btnModo3_Click(object sender, EventArgs e) => IniciarJuego(3);

        public GameSelectionForm(string username)
        {
            _usernameActual = username;
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
            return new Button { Tag = panel };
        }
        private async void IniciarJuego(int modo)
        {
            // 1. Feedback visual inmediato
            this.Cursor = Cursors.WaitCursor;
            btnModo1.Enabled = false; // Evitar doble clic
            btnModo2.Enabled = false;
            btnModo3.Enabled = false;

            int newGameId = 0;
            int idUsuario = 0;

            try
            {
                // 2. Ejecutar la BD en un hilo separado (NO BLOQUEA LA PANTALLA)
                await Task.Run(() =>
                {
                    using (var cn = conexion.ObtenerConexion())
                    {
                        cn.Open();

                        // Obtener ID Usuario
                        using (var cmdUser = new MySqlCommand("sp_ObtenerIdUsuario", cn))
                        {
                            cmdUser.CommandType = CommandType.StoredProcedure;
                            cmdUser.Parameters.AddWithValue("@p_Username", _usernameActual);
                            var res = cmdUser.ExecuteScalar();
                            if (res != null) idUsuario = Convert.ToInt32(res);
                        }

                        // Crear Partida
                        if (idUsuario > 0)
                        {
                            using (var cmd = new MySqlCommand("sp_IniciarJuego", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@p_IdUser", idUsuario);

                                // AGREGAR ESTA LÍNEA NUEVA:
                                cmd.Parameters.AddWithValue("@p_GameMode", modo);

                                var result = cmd.ExecuteScalar();
                                if (result != null) newGameId = Convert.ToInt32(result);
                            }
                        }
                    }
                });

                // 3. Ya volvió de la BD, abrimos el juego
                if (newGameId > 0)
                {
                    this.Hide();
                    GameForm game = new GameForm(modo, newGameId, idUsuario);
                    game.ShowDialog();
                    this.Show(); // Volver a mostrar menú al cerrar juego
                }
                else
                {
                    MessageBox.Show("Error al iniciar partida.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Restaurar controles
                this.Cursor = Cursors.Default;
                btnModo1.Enabled = true;
                btnModo2.Enabled = true;
                btnModo3.Enabled = true;
            }
        }
    }
}

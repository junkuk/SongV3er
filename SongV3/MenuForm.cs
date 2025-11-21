using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SongV3
{
    public partial class MenuForm : Form
    {
        private string username; // Para mostrar el nombre del usuario logueado
        private Label lblWelcome;
        private Button btnJugar;
        private Button btnHistorial;
        private Button btnEstadisticas;
        private Button btnConfiguracion;
        private Button btnSalir;

        public MenuForm(string user = "pen") // Recibe el nombre del usuario
        {
            username = user;
            InitializeComponent();
        
            this.Size = new System.Drawing.Size(1000, 600);
            this.Text = "Song Guesser - Menú Principal";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título de bienvenida
            lblWelcome = new Label
            {
                Text = $"¡Bienvenido, {username}!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 300) / 2, 60),
                ForeColor = Color.FromArgb(0, 122, 204),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblWelcome);

            // Subtítulo
            Label lblSubtitle = new Label
            {
                Text = "Elige una opción para comenzar",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 250) / 2, 100),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            Controls.Add(lblSubtitle);

            // Botones (centro vertical)
            int startY = 160;
            int buttonWidth = 300;
            int buttonHeight = 50;
            int spacing = 20;
            int centerX = (this.ClientSize.Width - buttonWidth) / 2;

            btnJugar = CreateMenuButton("Jugar", centerX, startY);
            btnJugar.Click += btnJugar_Click;

            btnHistorial = CreateMenuButton("Historial de Partidas", centerX, startY + (buttonHeight + spacing) * 1);
            btnHistorial.Click += btnHistorial_Click;

            btnEstadisticas = CreateMenuButton("Estadísticas", centerX, startY + (buttonHeight + spacing) * 2);
            btnEstadisticas.Click += btnEstadisticas_Click;

            btnConfiguracion = CreateMenuButton("Configuración", centerX, startY + (buttonHeight + spacing) * 3);
            btnConfiguracion.Click += btnConfiguracion_Click;

            btnSalir = CreateMenuButton("Salir", centerX, startY + (buttonHeight + spacing) * 4);
            btnSalir.BackColor = Color.FromArgb(220, 53, 69);
            btnSalir.Click += btnSalir_Click;

            Controls.Add(btnJugar);
            Controls.Add(btnHistorial);
            Controls.Add(btnEstadisticas);
            Controls.Add(btnConfiguracion);
            Controls.Add(btnSalir);
        }
        

        private Button CreateMenuButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private void btnJugar_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Se pasa el username como userId al constructor de GameSelectionForm
            var gameSelectionForm = new GameSelectionForm(username);
            gameSelectionForm.ShowDialog();
            this.Show();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            this.Hide();
            HistorialForm historial = new HistorialForm();
            historial.ShowDialog();
            this.Show();
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            this.Hide();
            EstadisticasForm stats = new EstadisticasForm();
            stats.ShowDialog();
            this.Show();
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            this.Hide();
            ConfiguracionForm config = new ConfiguracionForm();
            config.ShowDialog();
            this.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que quieres salir?", "Salir",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
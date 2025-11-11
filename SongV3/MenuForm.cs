using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SongV3
{
    public partial class MenuForm : Form
    {
        //aqui van las variables globales si es que hay
        private TextBox juego;
        public MenuForm()
        {
            InitializeComponent();
            // Si los controles no existen en el diseñador, agrégalos manualmente:


            this.Size = new System.Drawing.Size(1000, 600);
            this.Text = "Song Guesser - Menu principal";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245); // Fondo gris claro suave
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Button btnSeleccionDeJuego = new Button();
            btnSeleccionDeJuego.Name = "btnSeleccionDeJuego";
            btnSeleccionDeJuego.Text = "Selección de Juego";
            btnSeleccionDeJuego.Location = new System.Drawing.Point(20, 140); // Ajusta la ubicación según tu diseño
            btnSeleccionDeJuego.Click += new EventHandler(btnSeleccionDeJuego_Click);
            this.Controls.Add(btnSeleccionDeJuego);

        }
        private void MenuForm_Load(object sender, EventArgs e)
        {
            this.Text = "Menú Principal";
        }
        private void btnSeleccionDeJuego_Click(object sender, EventArgs e)
        {
            // Lógica para la selección de juego
            if (this.Visible)
            {
                this.Hide();
                GameSelectionForm gameSelectionForm = new GameSelectionForm();
                gameSelectionForm.ShowDialog();
                this.Show();
            }
        }
    }
}

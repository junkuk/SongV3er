namespace SongV3
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ConfigurarUI_SigueLaLetra()
        {
            //cambio de tamaño de form
            const int Margen = 80;
            int anchoUtil = this.ClientSize.Width - 2 * Margen;
            int centroX = Margen + anchoUtil / 2; // Centro real del área útil

            // lblTitulo
            lblTitulo = new Label
            {
                Text = "🎵 Sigue la Letra 🎵",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblTitulo.Location = new Point((this.ClientSize.Width - lblTitulo.Width) / 2, 40);
            Controls.Add(lblTitulo);

            // lblRonda
            lblRonda = new Label
            {
                Text = "Ronda: 1/10",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                Location = new Point(Margen, 100)
            };

            // lblScore
            lblScore = new Label
            {
                Text = "Puntos: 0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - Margen - 150, 100) // alineado a la derecha
            };
            Controls.Add(lblRonda);
            Controls.Add(lblScore);

            // lblLetraActual
            lblLetraActual = new Label
            {
                Text = "Cargando...",
                Font = new Font("Segoe UI", 18, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(Margen, 160),
                Size = new Size(anchoUtil, 100),
                Padding = new Padding(20)
            };
            Controls.Add(lblLetraActual);

            // btnReproducir
            btnReproducir = new Button
            {
                Text = "🔊 Reproducir fragmento",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(320, 60),
                Location = new Point(centroX - 160, 280),
                Cursor = Cursors.Hand
            };
            btnReproducir.FlatAppearance.BorderSize = 0;
            btnReproducir.Click += BtnReproducir_Click;
            Controls.Add(btnReproducir);

            // panelOpciones
            panelOpciones = new Panel
            {
                Location = new Point(Margen, 360),
                Size = new Size(anchoUtil, 220),
                BackColor = Color.FromArgb(245, 245, 250),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(panelOpciones);

            // RadioButtons para opciones
            opcionesRadio = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                opcionesRadio[i] = new RadioButton
                {
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.FromArgb(33, 33, 33),
                    Appearance = Appearance.Button,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(anchoUtil - 40, 50),
                    Location = new Point(20, 15 + i * 55),
                    Tag = i
                };
                // Habilita el botón "Verificar" automáticamente al seleccionar opción
                opcionesRadio[i].CheckedChanged += (s, e) =>
                {
                    btnSiguiente.Enabled = opcionesRadio.Any(r => r.Checked);
                };
                panelOpciones.Controls.Add(opcionesRadio[i]);
            }

            // Botón Verificar/Siguiente grande y perfectamente centrado
            btnSiguiente = new Button
            {
                Text = "Siguiente",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(320, 60),
                Location = new Point(centroX - 160, 600),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnSiguiente.FlatAppearance.BorderSize = 0;
            btnSiguiente.Click += BtnSiguiente_Click;
            Controls.Add(btnSiguiente);

            btnVerificar = new Button
            {
                Text = "Verificar",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(320, 60),
                Location = new Point(centroX - 160, 600),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnVerificar.FlatAppearance.BorderSize = 0;
            btnVerificar.Click += BtnVerificar_Click;
            Controls.Add(btnVerificar);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "GameForm";
        }

        #endregion
    }
}
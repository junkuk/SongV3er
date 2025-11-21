namespace SongV3
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "MenuForm";
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

        #endregion
    }
}

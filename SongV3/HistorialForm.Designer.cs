namespace SongV3
{
    partial class HistorialForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGlobal;
        private System.Windows.Forms.TabPage tabPersonal;
        private System.Windows.Forms.DataGridView dgvGlobal;
        private System.Windows.Forms.DataGridView dgvPersonal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Configuración del Formulario
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Text = "Song Guesser - Historial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTitulo.Text = "🏆 Historial y Rankings";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(300, 20);

            // TabControl (Pestañas)
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabControl1.Location = new System.Drawing.Point(50, 80);
            this.tabControl1.Size = new System.Drawing.Size(800, 420);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 10F);

            // Pestaña 1: Global
            this.tabGlobal = new System.Windows.Forms.TabPage();
            this.tabGlobal.Text = "Top 10 Global";
            this.tabGlobal.BackColor = System.Drawing.Color.White;

            this.dgvGlobal = new System.Windows.Forms.DataGridView();
            this.dgvGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabGlobal.Controls.Add(this.dgvGlobal);

            // Pestaña 2: Personal
            this.tabPersonal = new System.Windows.Forms.TabPage();
            this.tabPersonal.Text = "Mis Partidas";
            this.tabPersonal.BackColor = System.Drawing.Color.White;

            this.dgvPersonal = new System.Windows.Forms.DataGridView();
            this.dgvPersonal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPersonal.Controls.Add(this.dgvPersonal);

            this.tabControl1.Controls.Add(this.tabGlobal);
            this.tabControl1.Controls.Add(this.tabPersonal);

            // Botón Volver
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnVolver.Text = "Volver al Menú";
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnVolver.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnVolver.ForeColor = System.Drawing.Color.White;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.FlatAppearance.BorderSize = 0;
            this.btnVolver.Size = new System.Drawing.Size(150, 40);
            this.btnVolver.Location = new System.Drawing.Point(375, 520);
            this.btnVolver.Click += (s, e) => this.Close();

            // Agregar controles al Form
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnVolver);

            // Evento Load
            this.Load += new System.EventHandler(this.HistorialForm_Load);
        }
    }
}
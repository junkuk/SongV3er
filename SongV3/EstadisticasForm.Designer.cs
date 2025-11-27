namespace SongV3
{
    partial class EstadisticasForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlKpiPartidas;
        private System.Windows.Forms.Label lblValPartidas;
        private System.Windows.Forms.Label lblTitPartidas;
        private System.Windows.Forms.Panel pnlKpiRecord;
        private System.Windows.Forms.Label lblValRecord;
        private System.Windows.Forms.Label lblTitRecord;
        private System.Windows.Forms.Panel pnlKpiPromedio;
        private System.Windows.Forms.Label lblValPromedio;
        private System.Windows.Forms.Label lblTitPromedio;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRendimiento;
        private System.Windows.Forms.GroupBox grpLogros;
        private System.Windows.Forms.DataGridView dgvLogros;
        private System.Windows.Forms.GroupBox grpMongo;
        private System.Windows.Forms.ListBox lstMongoLogs;
        private System.Windows.Forms.Button btnVolver;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();

            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlKpiPartidas = new System.Windows.Forms.Panel();
            this.lblValPartidas = new System.Windows.Forms.Label();
            this.lblTitPartidas = new System.Windows.Forms.Label();
            this.pnlKpiRecord = new System.Windows.Forms.Panel();
            this.lblValRecord = new System.Windows.Forms.Label();
            this.lblTitRecord = new System.Windows.Forms.Label();
            this.pnlKpiPromedio = new System.Windows.Forms.Panel();
            this.lblValPromedio = new System.Windows.Forms.Label();
            this.lblTitPromedio = new System.Windows.Forms.Label();
            this.chartRendimiento = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.grpLogros = new System.Windows.Forms.GroupBox();
            this.dgvLogros = new System.Windows.Forms.DataGridView();
            this.grpMongo = new System.Windows.Forms.GroupBox();
            this.lstMongoLogs = new System.Windows.Forms.ListBox();
            this.btnVolver = new System.Windows.Forms.Button();

            // Form
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Text = "Dashboard de Rendimiento";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            // Título
            this.lblTitulo.Text = "📊 Dashboard del Jugador";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.lblTitulo.Location = new System.Drawing.Point(30, 20);
            this.lblTitulo.AutoSize = true;

            // --- TARJETAS KPI ---
            // KPI 1: Partidas
            this.pnlKpiPartidas.BackColor = System.Drawing.Color.White;
            this.pnlKpiPartidas.Location = new System.Drawing.Point(30, 70);
            this.pnlKpiPartidas.Size = new System.Drawing.Size(300, 100);
            this.pnlKpiPartidas.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this.lblValPartidas.Text = "0";
            this.lblValPartidas.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblValPartidas.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lblValPartidas.Location = new System.Drawing.Point(0, 40);
            this.lblValPartidas.Size = new System.Drawing.Size(300, 50);
            this.lblValPartidas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblTitPartidas.Text = "TOTAL PARTIDAS";
            this.lblTitPartidas.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitPartidas.ForeColor = System.Drawing.Color.Gray;
            this.lblTitPartidas.Location = new System.Drawing.Point(0, 10);
            this.lblTitPartidas.Size = new System.Drawing.Size(300, 20);
            this.lblTitPartidas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlKpiPartidas.Controls.Add(this.lblValPartidas);
            this.pnlKpiPartidas.Controls.Add(this.lblTitPartidas);

            // KPI 2: Récord
            this.pnlKpiRecord.BackColor = System.Drawing.Color.White;
            this.pnlKpiRecord.Location = new System.Drawing.Point(350, 70);
            this.pnlKpiRecord.Size = new System.Drawing.Size(300, 100);

            this.lblValRecord.Text = "0";
            this.lblValRecord.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblValRecord.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113); // Verde
            this.lblValRecord.Location = new System.Drawing.Point(0, 40);
            this.lblValRecord.Size = new System.Drawing.Size(300, 50);
            this.lblValRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblTitRecord.Text = "PUNTUACIÓN MÁXIMA";
            this.lblTitRecord.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitRecord.ForeColor = System.Drawing.Color.Gray;
            this.lblTitRecord.Location = new System.Drawing.Point(0, 10);
            this.lblTitRecord.Size = new System.Drawing.Size(300, 20);
            this.lblTitRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlKpiRecord.Controls.Add(this.lblValRecord);
            this.pnlKpiRecord.Controls.Add(this.lblTitRecord);

            // KPI 3: Promedio
            this.pnlKpiPromedio.BackColor = System.Drawing.Color.White;
            this.pnlKpiPromedio.Location = new System.Drawing.Point(670, 70);
            this.pnlKpiPromedio.Size = new System.Drawing.Size(300, 100);

            this.lblValPromedio.Text = "0.0";
            this.lblValPromedio.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblValPromedio.ForeColor = System.Drawing.Color.FromArgb(155, 89, 182); // Morado
            this.lblValPromedio.Location = new System.Drawing.Point(0, 40);
            this.lblValPromedio.Size = new System.Drawing.Size(300, 50);
            this.lblValPromedio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblTitPromedio.Text = "PROMEDIO PUNTOS";
            this.lblTitPromedio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitPromedio.ForeColor = System.Drawing.Color.Gray;
            this.lblTitPromedio.Location = new System.Drawing.Point(0, 10);
            this.lblTitPromedio.Size = new System.Drawing.Size(300, 20);
            this.lblTitPromedio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlKpiPromedio.Controls.Add(this.lblValPromedio);
            this.pnlKpiPromedio.Controls.Add(this.lblTitPromedio);

            // --- GRÁFICA (CHART) ---
            chartArea1.Name = "ChartArea1";
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);

            this.chartRendimiento.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            this.chartRendimiento.Legends.Add(legend1);
            this.chartRendimiento.Location = new System.Drawing.Point(30, 190);
            this.chartRendimiento.Size = new System.Drawing.Size(940, 200);
            this.chartRendimiento.Text = "chart1";

            // Serie de datos
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Historial de Puntos";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea; // Gráfica de Área Suave
            series1.Color = System.Drawing.Color.FromArgb(100, 0, 122, 204); // Azul semitransparente
            series1.BorderColor = System.Drawing.Color.FromArgb(0, 122, 204);
            series1.BorderWidth = 2;
            this.chartRendimiento.Series.Add(series1);

            // --- LOGROS ---
            this.grpLogros.Text = "🏆 Logros Desbloqueados";
            this.grpLogros.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpLogros.Location = new System.Drawing.Point(30, 410);
            this.grpLogros.Size = new System.Drawing.Size(460, 220);
            this.grpLogros.BackColor = System.Drawing.Color.White;

            this.dgvLogros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLogros.BackgroundColor = System.Drawing.Color.White;
            this.dgvLogros.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLogros.RowHeadersVisible = false;
            this.dgvLogros.AllowUserToAddRows = false;
            this.dgvLogros.ReadOnly = true;
            this.dgvLogros.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grpLogros.Controls.Add(this.dgvLogros);

            // --- MONGO CONSOLE ---
            this.grpMongo.Text = "🍃 Logs de Sistema (MongoDB Atlas)";
            this.grpMongo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpMongo.Location = new System.Drawing.Point(510, 410);
            this.grpMongo.Size = new System.Drawing.Size(460, 220);
            this.grpMongo.BackColor = System.Drawing.Color.White;

            this.lstMongoLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMongoLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstMongoLogs.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.lstMongoLogs.ForeColor = System.Drawing.Color.FromArgb(0, 255, 0);
            this.grpMongo.Controls.Add(this.lstMongoLogs);

            // Botón Volver
            this.btnVolver.Text = "Cerrar";
            this.btnVolver.Location = new System.Drawing.Point(850, 650);
            this.btnVolver.Size = new System.Drawing.Size(120, 35);
            this.btnVolver.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnVolver.ForeColor = System.Drawing.Color.White;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Click += (s, e) => this.Close();

            // Agregar al Form
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnlKpiPartidas);
            this.Controls.Add(this.pnlKpiRecord);
            this.Controls.Add(this.pnlKpiPromedio);
            this.Controls.Add(this.chartRendimiento);
            this.Controls.Add(this.grpLogros);
            this.Controls.Add(this.grpMongo);
            this.Controls.Add(this.btnVolver);
        }
    }
}
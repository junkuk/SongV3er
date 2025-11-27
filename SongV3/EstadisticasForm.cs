using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SongV3
{
    public partial class EstadisticasForm : Form
    {
        private int currentUserId;

        public EstadisticasForm(int userId)
        {
            this.currentUserId = userId;
            InitializeComponent();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarDatosSQL();
            CargarGraficaHistorial();
            await CargarDatosMongo();
        }

        private void CargarDatosSQL()
        {
            try
            {
                using (var cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    using (var cmd = new MySqlCommand("sp_ObtenerPerfilUsuario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_IdUser", currentUserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblValPartidas.Text = reader["TotalPartidas"].ToString();
                                lblValRecord.Text = reader["MaxPuntuacion"].ToString();
                                lblValPromedio.Text = reader["PromedioPuntos"].ToString();
                            }

                            if (reader.NextResult())
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);
                                dgvLogros.DataSource = dt;

                                if (dgvLogros.Columns.Contains("Condition_Score"))
                                    dgvLogros.Columns["Condition_Score"].Visible = false;

                                dgvLogros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
        }

        private void CargarGraficaHistorial()
        {
            try
            {
                using (var cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    using (var cmd = new MySqlCommand("sp_ObtenerHistorialUsuario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_IdUser", currentUserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var serie = chartRendimiento.Series[0];
                            serie.Points.Clear();

                            serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

                            serie.BorderWidth = 3;
                            serie.Color = Color.FromArgb(0, 122, 204);
                            serie.ShadowOffset = 2;

                            serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                            serie.MarkerSize = 10;
                            serie.MarkerColor = Color.White;
                            serie.MarkerBorderColor = Color.FromArgb(0, 122, 204);
                            serie.MarkerBorderWidth = 2;

                            serie.IsValueShownAsLabel = true;
                            serie.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                            serie.LabelForeColor = Color.DimGray;

                            var area = chartRendimiento.ChartAreas[0];
                            area.AxisY.Minimum = 0;
                            area.AxisY.Maximum = 100;

                            area.AxisX.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
                            area.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
                            area.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
                            area.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;

                            List<int> scores = new List<int>();
                            while (reader.Read())
                            {
                                scores.Add(Convert.ToInt32(reader["Puntos"]));
                            }
                            scores.Reverse();

                            int partidaN = 1;
                            foreach (int score in scores)
                            {
                                serie.Points.AddXY($"P{partidaN}", score);
                                partidaN++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error gráfica: " + ex.Message);
            }
        }

        private async Task CargarDatosMongo()
        {
            lstMongoLogs.Items.Add("> Iniciando conexión a Atlas...");

            try
            {
                var logs = await MongoLogger.GetRecentLogsAsync(currentUserId.ToString(), 15);

                lstMongoLogs.Items.Clear();
                if (logs.Count == 0)
                {
                    lstMongoLogs.Items.Add("> No se encontraron logs recientes.");
                }
                else
                {
                    foreach (var log in logs)
                    {
                        lstMongoLogs.Items.Add($"> {log}");
                    }
                }
            }
            catch (Exception ex)
            {
                lstMongoLogs.Items.Add("> Error: " + ex.Message);
            }
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SongV3
{
    public partial class HistorialForm : Form
    {
        private int currentUserId;

        // Constructor que recibe el ID del usuario
        public HistorialForm(int userId)
        {
            this.currentUserId = userId;
            InitializeComponent();
            PersonalizarDiseñoGrid();
        }

        private void HistorialForm_Load(object sender, EventArgs e)
        {
            CargarLeaderboard();
            CargarHistorialPersonal();
        }

        private void PersonalizarDiseñoGrid()
        {
            // Estilo visual para ambas tablas (Global y Personal)
            ConfigurarEstilo(dgvGlobal);
            ConfigurarEstilo(dgvPersonal);
        }

        private void ConfigurarEstilo(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;

            // Encabezado Azul
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;

            // Filas
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargarLeaderboard()
        {
            try
            {
                using (var cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    using (var cmd = new MySqlCommand("sp_ObtenerLeaderboard", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvGlobal.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ranking: " + ex.Message);
            }
        }

        private void CargarHistorialPersonal()
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

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvPersonal.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial personal: " + ex.Message);
            }
        }
    }
}
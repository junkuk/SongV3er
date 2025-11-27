using Microsoft.VisualBasic.Logging;
using MySql.Data.MySqlClient;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace SongV3
{
    public partial class GameForm : Form
    {
        private int modoJuego;
        private int newGame;
        private string? username;
        private int userId;
        private int rondaActual = 0;
        private const int rondasTotales = 10;
        private int currentGameId = -1;

        // Audio
        private WaveOutEvent? outputDevice;
        private WaveStream? audioFile;
        private System.Windows.Forms.Timer? audioTimer;

        // Datos ronda actual
        private int currentSongId;
        private string? correctLyric;
        private string? songFilePath;
        private string? titleSong;
        private string? artist;
        private double startTime;
        private string respuestaCorrectaTexto;

        // UI
        private Label? lblTitulo, lblRonda, lblScore, lblLetraActual;
        private Button? btnReproducir, btnSiguiente, btnVerificar;
        private RadioButton[]? opcionesRadio;
        private Panel? panelOpciones;
        public GameForm(int modo, int gameId, int idUser)
        {
            modoJuego = modo;
            currentGameId = gameId;
            userId = idUser;

            InitializeComponent();

            if (modo == 1) ConfigurarUI_SigueLaLetra();
            else if (modo == 2) ConfigurarUI_AdivinaTituloyArtista();
            else ConfigurarUI_AdivinaArtista();

            ControlarUI(true);

            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 521);
            ForeColor = Color.Black;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            ResumeLayout(false);
            PerformLayout();
        }
        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Application.DoEvents();

            if (lblLetraActual != null) lblLetraActual.Text = "Conectando con base de datos...";

            await Task.Delay(10);

            CargarSiguienteRonda();
        }
        private void CargarSiguienteRonda()
        {
            DetenerAudio();
            if (rondaActual >= rondasTotales)
            {
                FinalizarJuego();
                return;
            }

            rondaActual++;
            if (lblRonda != null) lblRonda.Text = $"Ronda: {rondaActual}/{rondasTotales}";

            if (opcionesRadio != null)
            {
                foreach (var rb in opcionesRadio)
                {
                    rb.Checked = false;
                    rb.ForeColor = Color.Black;
                    rb.Enabled = true;
                    rb.Text = "Cargando...";
                    rb.Tag = null;
                }
            }

            // Aseguramos que el botón Siguiente se oculte y Verificar se muestre (aunque esté disabled)
            if (btnSiguiente != null) btnSiguiente.Visible = false;
            if (btnVerificar != null)
            {
                btnVerificar.Visible = true;
                btnVerificar.Enabled = false; // Debe estar false hasta que el usuario elija algo
                btnVerificar.Text = "✅ Verificar Respuesta";
            }

            try
            {
                using (var cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    using (var cmd = new MySqlCommand("sp_ObtenerDatosRonda", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_Modo", modoJuego);
                        cmd.Parameters.AddWithValue("@p_IdGame", currentGameId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            // --- RESULT SET 1: CANCIÓN ---
                            if (reader.Read())
                            {
                                currentSongId = reader.GetInt32("Id_Song");
                                songFilePath = reader["File_Path"].ToString();
                                startTime = reader.IsDBNull(reader.GetOrdinal("Preview_Start")) ? 30 : reader.GetDouble("Preview_Start");
                                respuestaCorrectaTexto = reader["CorrectAnswer"].ToString();

                                if (lblLetraActual != null)
                                    lblLetraActual.Text = (modoJuego == 1) ? $"Completa: {reader["Title"]}" : "Escucha y adivina...";
                            }
                            else
                            {
                                MessageBox.Show("¡Juego completado o sin más canciones!");
                                FinalizarJuego();
                                return;
                            }

                            // --- RESULT SET 2: OPCIONES ---
                            if (reader.NextResult())
                            {
                                int i = 0;
                                while (reader.Read() && i < 4)
                                {
                                    if (opcionesRadio != null && opcionesRadio[i] != null)
                                    {
                                        string textoOpcion = reader["OpcionTexto"].ToString();

                                        // Protección extra por si llega vacío desde BD
                                        if (string.IsNullOrWhiteSpace(textoOpcion))
                                            textoOpcion = "(Opción vacía - Error en BD)";

                                        opcionesRadio[i].Text = textoOpcion;
                                        opcionesRadio[i].Tag = reader.GetInt32("EsCorrecta");
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                }
                // Habilitar reproducción ahora que tenemos datos
                if (btnReproducir != null) btnReproducir.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando ronda: " + ex.Message);
            }
        }
        private async void BtnReproducir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(songFilePath)) return;

            // Aseguramos limpieza total antes de empezar
            DetenerAudio();

            btnReproducir.Enabled = false;

            try
            {
                var youtube = new YoutubeClient();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(songFilePath);

                var streamInfo = streamManifest
                    .GetAudioOnlyStreams()
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("No se encontró audio.");

                string audioUrl = streamInfo.Url;

                // Crear stream y dispositivo
                audioFile = new MediaFoundationReader(audioUrl);
                if (startTime > 0)
                    audioFile.CurrentTime = TimeSpan.FromSeconds(startTime);

                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // --- AQUÍ EL CAMBIO IMPORTANTE ---
                // Usamos la variable global audioTimer
                audioTimer = new System.Windows.Forms.Timer { Interval = 7000 }; // 7 segundos
                audioTimer.Tick += (s, ev) =>
                {
                    // Cuando pasen los 7 seg, detenemos todo
                    DetenerAudio();
                    if (btnReproducir != null && !btnReproducir.IsDisposed)
                        btnReproducir.Enabled = true;
                };
                audioTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DetenerAudio(); // Limpiar si falló
                if (btnReproducir != null) btnReproducir.Enabled = true;
            }
        }
        private void DetenerAudio()
        {
            // 1. Matar el Timer primero (para que no intente detener algo que ya borramos)
            if (audioTimer != null)
            {
                audioTimer.Stop();
                audioTimer.Dispose();
                audioTimer = null;
            }

            // 2. Detener la reproducción
            if (outputDevice != null)
            {
                try
                {
                    if (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        outputDevice.Stop();
                    }
                    outputDevice.Dispose();
                }
                catch { /* Ignorar errores al detener */ }
                finally
                {
                    outputDevice = null;
                }
            }

            // 3. Liberar el archivo de audio (Stream)
            if (audioFile != null)
            {
                try
                {
                    audioFile.Dispose();
                }
                catch { }
                finally
                {
                    audioFile = null;
                }
            }
        }
        private async void BtnVerificar_Click(object sender, EventArgs e)
        {
            DetenerAudio();
            var seleccionada = opcionesRadio?.FirstOrDefault(r => r.Checked);
            if (seleccionada == null) return;

            this.Cursor = Cursors.WaitCursor;
            if (btnVerificar != null) btnVerificar.Enabled = false;

            int esCorrecta = (seleccionada.Tag != null && Convert.ToInt32(seleccionada.Tag) == 1) ? 1 : 0;
            string respuestaTexto = seleccionada.Text;

            int puntosGanados = 0;
            int scoreTotal = 0;
            string? logro = null;
            bool operacionExitosa = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var cn = conexion.ObtenerConexion())
                    {
                        cn.Open();
                        using (var cmd = new MySqlCommand("sp_ProcesarRondaUsuario", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@p_IdGame", currentGameId);
                            cmd.Parameters.AddWithValue("@p_IdSong", currentSongId);
                            cmd.Parameters.AddWithValue("@p_RespuestaUsuario", respuestaTexto);
                            cmd.Parameters.AddWithValue("@p_EsCorrecta", esCorrecta);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    puntosGanados = reader.GetInt32("PuntosGanados");
                                    scoreTotal = reader.GetInt32("ScoreTotal");
                                    logro = reader.IsDBNull(reader.GetOrdinal("LogroDesbloqueado")) ? null : reader.GetString("LogroDesbloqueado");
                                    operacionExitosa = true;
                                }
                            }
                        }
                    }
                });

                if (operacionExitosa)
                {
                    if (lblScore != null) lblScore.Text = $"Puntos: {scoreTotal}";
                    DarFeedbackVisual(esCorrecta == 1, puntosGanados);

                    if (!string.IsNullOrEmpty(logro))
                    {
                        MessageBox.Show($"🏆 ¡LOGRO DESBLOQUEADO!\nHas conseguido: {logro}", "Achievement Unlocked");
                    }
                }

                if (operacionExitosa && !string.IsNullOrEmpty(logro))
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {                       
                            MongoLogger.LogAction(userId.ToString(), "Achievement", logro);
                        }
                        catch {}
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                ControlarUI(false);
            }
        }
        private void BtnSiguiente_Click(object? sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing) return;

            CargarSiguienteRonda();
        }
        private void DarFeedbackVisual(bool acerto, int puntos)
        {
            // Colorear botones
            if (opcionesRadio != null)
            {
                foreach (var rb in opcionesRadio)
                {
                    rb.Enabled = false;
                    // Si tiene Tag=1 es la correcta -> Verde
                    if (rb.Tag != null && Convert.ToInt32(rb.Tag) == 1)
                        rb.ForeColor = Color.Green;
                    // Si fue la seleccionada y falló -> Rojo
                    else if (rb.Checked && !acerto)
                        rb.ForeColor = Color.Red;
                }
            }

            if (acerto)
                MessageBox.Show($"🎉 ¡Correcto! +{puntos} puntos");
            else
                MessageBox.Show($"Incorrecto 😢. Era: {respuestaCorrectaTexto}");
        }
        private void FinalizarJuego()
        {

            MessageBox.Show($"¡Juego terminado!\nPuntuación final: {lblScore?.Text.Replace("Puntos: ", "") ?? "0"}", "¡Genial!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _ = Task.Run(() =>
            {
                try
                {
                    using (var cn = conexion.ObtenerConexion())
                    {
                        cn.Open();
                        string query = "CALL sp_FinalizarJuego(@p_IdGame)";
                        using (var cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@p_IdGame", currentGameId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MongoLogger.LogAction(userId.ToString(), "EndGame", "Partida finalizada");
                }
                catch { }
            });

            this.Close();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DetenerAudio();
            base.OnFormClosing(e);
        }
        private void ControlarUI(bool esRondaActiva)
        {
            if (esRondaActiva)
            {
                if (btnVerificar != null)
                {
                    btnVerificar.Enabled = true;
                    btnVerificar.Visible = true;
                    btnVerificar.Text = "✅ Verificar Respuesta";
                }
                if (btnSiguiente != null)
                {
                    btnSiguiente.Enabled = false;
                    btnSiguiente.Visible = false;
                }
            }
            else
            {
                if (btnVerificar != null)
                {
                    btnVerificar.Enabled = false;
                    btnVerificar.Visible = false;
                }
                if (btnSiguiente != null)
                {
                    btnSiguiente.Enabled = true;
                    btnSiguiente.Visible = true;
                    btnSiguiente.Text = "➡️ Siguiente Ronda";
                }
            }
        }
    }
}
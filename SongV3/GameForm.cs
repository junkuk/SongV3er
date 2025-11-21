using MySql.Data.MySqlClient;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
        private int userId; // Para guardar en rounds y games
        private int rondaActual = 0;
        private int score = 0;
        private const int rondasTotales = 10;
        private int currentGameId = -1;

        // Audio
        private WaveOutEvent? outputDevice;
        private WaveStream? audioFile;

        // Datos ronda actual
        private int currentSongId;
        private string? correctLyric;
        private string? songFilePath;
        private string? titleSong;
        private double startTime;

        // UI
        private Label? lblTitulo, lblRonda, lblScore, lblLetraActual;
        private Button? btnReproducir, btnSiguiente, btnVerificar;
        private RadioButton[]? opcionesRadio;
        private Panel? panelOpciones;


        // cosas a hacer:
        // 1.- Se necesita que no se repitan por ronda las canciones
        // 2.- 
        // 3.- Que se espere al terminar ronda para cargar el audio de la siguiente
        // 4.- Que se pueda pausar el audio (boton pausar) y que se pueda reproducir de nuevo (boton reproducir)
        // 5.- Que se pueda configurar el numero de rondas al iniciar el juego
        // 6.- Estadististicas de usuario al inciio de forms

        public GameForm(int modo, int gameG, int idUsuario)
        {
            modoJuego = modo;
            newGame = gameG;
            userId= idUsuario;

            InitializeComponent();

            this.Size = new Size(1100, 700);
            this.Text = "Song Guesser - Sigue la Letra";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            if (modoJuego != 1)
            {
                MessageBox.Show("Modo no implementado aún");
                this.Close();
                return;
            }

            ConfigurarUI_SigueLaLetra();
            currentGameId = gameG;
            CargarSiguienteRonda();
        }
        private void CargarSiguienteRonda()
        {
            if (rondaActual >= rondasTotales)
            {
                FinalizarJuego();
                return;
            }

            rondaActual++;
            if (lblRonda != null && !lblRonda.IsDisposed)
                lblRonda.Text = $"Ronda: {rondaActual}/{rondasTotales}";

            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    // 1. Canción + letra + Preview_Start
                    string query = @"
                SELECT 
                    l.Lyric_Text,
                    s.Id_Song,
                    s.File_Path,
                    s.Title,
                    s.Artist,
                    s.Preview_Start
                FROM songguesser_lyrics l
                INNER JOIN songs s ON l.Id_Song = s.Id_Song
                ORDER BY RAND() 
                LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("No hay letras disponibles en la base de datos");
                            FinalizarJuego();
                            return;
                        }

                        correctLyric = reader["Lyric_Text"]?.ToString() ?? string.Empty;
                        currentSongId = Convert.ToInt32(reader["Id_Song"]);
                        songFilePath = reader["File_Path"]?.ToString() ?? string.Empty;
                        titleSong = reader["Title"]?.ToString() ?? string.Empty;

                        // ← Aquí está la línea clave que querías
                        startTime = reader.GetDouble("Preview_Start");  
                        if (string.IsNullOrEmpty(correctLyric))
                            lblLetraActual.Text = "Letra no disponible";
                        else
                           lblLetraActual.Text = $"🎤 Escucha y completa: {titleSong}";
                        
                    }

                    // 2. Distractores (3 letras falsas)
                    cn.Close();
                    cn.Open();

                    query = @"
                SELECT Lyric_Text 
                FROM songguesser_lyrics 
                WHERE Id_Song != @id 
                ORDER BY RAND() 
                LIMIT 3";

                    List<string> distractors = new List<string>();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@id", currentSongId);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                                distractors.Add(r["Lyric_Text"]?.ToString() ?? "");
                        }
                    }

                    if (distractors.Count < 3)
                    {
                        MessageBox.Show("No hay suficientes letras para distractores");
                        FinalizarJuego();
                        return;
                    }

                    // Mezclar opciones
                    string[] opciones = new string[4];
                    Random rnd = new Random();
                    int posCorrecta = rnd.Next(0, 4);
                    opciones[posCorrecta] = correctLyric;

                    int idx = 0;
                    for (int i = 0; i < 4; i++)
                        if (i != posCorrecta)
                            opciones[i] = distractors[idx++];

                    // Asignar a RadioButtons
                    for (int i = 0; i < 4; i++)
                    {
                        var rb = opcionesRadio[i];
                        rb.Text = opciones[i];
                        rb.Checked = false;
                        rb.Enabled = true;
                        rb.ForeColor = Color.FromArgb(50, 50, 50);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando ronda: " + ex.Message);
                FinalizarJuego();
            }

            btnReproducir.Enabled = true;
           // btnSiguiente.Enabled = false;
            // btnSiguiente.Text = "✅ Verificar Respuesta";

            btnVerificar.Click -= BtnVerificar_Click;
            btnVerificar.Click += BtnVerificar_Click;
            btnVerificar.Enabled = true;
            btnVerificar.Text = "✅ Verificar Respuesta";
        }

        private async void BtnReproducir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(songFilePath)) return;

            btnReproducir.Enabled = false;
            DetenerAudio();

            try
            {
                var youtube = new YoutubeClient();

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(songFilePath);

                var streamInfo = streamManifest
                    .GetAudioOnlyStreams()
                    .FirstOrDefault(s => s.Container.Name.Equals("mp4", StringComparison.OrdinalIgnoreCase))
                    ?? streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("No se encontró stream de audio compatible");

                // URL directa del audio puro
                string audioUrl = streamInfo.Url;

                // NAudio MediaFoundationReader acepta la URL directa
                audioFile = new MediaFoundationReader(audioUrl);

                // Opcional: empezar en el segundo que quieras (ej. 30 seg para saltar intro)
                if (startTime > 0)
                    audioFile.CurrentTime = TimeSpan.FromSeconds(startTime);

                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Detener automáticamente a los 6 segundos
                var timer = new System.Windows.Forms.Timer { Interval = 6000 };
                timer.Tick += (s, ev) =>
                {
                    DetenerAudio();
                    btnReproducir.Enabled = true;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reproduciendo YouTube: {ex.Message}\n", // aqui qpdo lit borre "\nNo tienes conexion a internet" y jalo jaja  
                               "Error de audio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnReproducir.Enabled = true;
            }
        }

        private void DetenerAudio()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }
        
        private void BtnVerificar_Click(object sender, EventArgs e)
        {
            RadioButton? seleccionada = opcionesRadio?.FirstOrDefault(r => r.Checked);
            if (seleccionada == null)
            {
                MessageBox.Show("¡Selecciona una opción!");
                return;
            }
            bool esCorrecta = seleccionada.Text == (correctLyric ?? string.Empty);
            int puntos = esCorrecta ? 10 : 0;
            score += puntos;

            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    // Cambiar por Stored Procedure
                    string query = "INSERT INTO rounds (Id_Game, Id_Song, Guessed_title, Guessed_artist, Point_awarded) VALUES (@game, @song, 0, 0, @puntos)";
                    MySqlCommand cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@game", currentGameId);
                    cmd.Parameters.AddWithValue("@song", currentSongId);
                    cmd.Parameters.AddWithValue("@puntos", puntos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            if (opcionesRadio != null)
            {
                foreach (RadioButton rb in opcionesRadio)
                {
                    if (rb == null) continue;
                    if (!rb.IsDisposed) rb.Enabled = false;
                    if (rb.Text == correctLyric) if (!rb.IsDisposed) rb.ForeColor = Color.Green;
                    if (rb.Checked && !esCorrecta) if (!rb.IsDisposed) rb.ForeColor = Color.Red;
                }
            }
            if (lblScore != null && !lblScore.IsDisposed) lblScore.Text = $"Puntos: {score}";
            MessageBox.Show(esCorrecta ? "¡CORRECTO! +10" : $"Incorrecto.\nEra: {correctLyric}");

            // Reemplazar el handler actual por la acción de cargar la siguiente ronda de forma segura
            if (btnVerificar != null && !btnVerificar.IsDisposed)
            {

                try
                {
                    btnVerificar.Click -= BtnVerificar_Click;
                }
                catch { }
                btnVerificar.Text = "➡️ verificar respuesta";
                btnVerificar.Click += (s, ev) =>
                {
                    // Si el formulario ya fue disposed no ejecutar
                    if (this.IsDisposed || this.Disposing) return;
                    // No carga la siguiente ronda, se crea un nuevo handler BtnSiguiente_Click
                    // aqui carga el boton siguiente ronda
                    btnSiguiente.Enabled = true;
                    btnSiguiente.Text = "Siguiente ronda";
                };
            }
        }

        private void BtnSiguiente_Click(object? sender, EventArgs e)
        {
            RadioButton? seleccionada = opcionesRadio?.FirstOrDefault(r => r.Checked);
            if (seleccionada == null)
            {
                MessageBox.Show("¡Selecciona una opción!");
                return;
            }

            bool esCorrecta = seleccionada.Text == (correctLyric ?? string.Empty);
            int puntos = esCorrecta ? 10 : 0;
            score += puntos;

            // Guardar ronda
            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    // Cambiar por Stored Procedure
                    string query = "INSERT INTO rounds (Id_Game, Id_Song, Guessed_title, Guessed_artist, Point_awarded) VALUES (@game, @song, 0, 0, @puntos)";
                    MySqlCommand cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@game", currentGameId);
                    cmd.Parameters.AddWithValue("@song", currentSongId);
                    cmd.Parameters.AddWithValue("@puntos", puntos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

            // Feedback
            if (opcionesRadio != null)
            {
                foreach (RadioButton rb in opcionesRadio)
                {
                    if (rb == null) continue;
                    if (!rb.IsDisposed) rb.Enabled = false;
                    if (rb.Text == correctLyric) if (!rb.IsDisposed) rb.ForeColor = Color.Green;
                    if (rb.Checked && !esCorrecta) if (!rb.IsDisposed) rb.ForeColor = Color.Red;
                }
            }
            if (lblScore != null && !lblScore.IsDisposed) lblScore.Text = $"Puntos: {score}";
            MessageBox.Show(esCorrecta ? "¡CORRECTO! +10" : $"Incorrecto.\nEra: {correctLyric}");

            // Reemplazar el handler actual por la acción de cargar la siguiente ronda de forma segura
            if (btnSiguiente != null && !btnSiguiente.IsDisposed)
            {
                
                try
                {
                    btnSiguiente.Click -= BtnSiguiente_Click;
                }
                catch { }
                btnSiguiente.Text = "➡️ Siguiente";
                btnSiguiente.Click += (s, ev) =>
                {
                    // Si el formulario ya fue disposed no ejecutar
                    if (this.IsDisposed || this.Disposing) return;
                    CargarSiguienteRonda();
                    
                };
            }
        }

        private void FinalizarJuego()
        {
            // Actualizar score final
            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    string query = "UPDATE games SET Score = @score WHERE Id_Game = @id";
                    MySqlCommand cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.Parameters.AddWithValue("@id", currentGameId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

            MessageBox.Show($"¡Juego terminado!\nPuntuación final: {score}", "¡Genial!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DetenerAudio();
            base.OnFormClosing(e);
        }
    }
}

using MySql.Data.MySqlClient;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
        private int score = 0;
        private int puntosLetra = 0;
        private int puntosCancionArtista = 0;
        private int puntosCancion = 0;
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
        private string? artist;
        private double startTime;

        // UI
        private Label? lblTitulo, lblRonda, lblScore, lblLetraActual;
        private Button? btnReproducir, btnSiguiente, btnVerificar;
        private RadioButton[]? opcionesRadio;
        private Panel? panelOpciones;


        // cosas a hacer:
        // 1.- Se necesita que no se repitan por ronda las canciones
        // 2.- Que la dificultad sea por canciones no tan populares 
        // 3.- Que se espere al terminar ronda para cargar el audio de la siguiente
        // 4.- Que se pueda pausar el audio (boton pausar) y que se pueda reproducir de nuevo (boton reproducir)
        // 5.- Que se pueda configurar el numero de rondas al iniciar el juego
        // 6.- Guardar el audio de la cancion en la RAM despues de haber cargado el juego para que no tarde en cargar cada ronda

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
            if (modoJuego == 1)
            {
                ConfigurarUI_SigueLaLetra();
                currentGameId = gameG;
                ControlarUI(true);
                CargarSiguienteRonda();
            }
            if (modoJuego == 2)
            {
              
                ConfigurarUI_AdivinaTituloyArtista();              
                currentGameId = gameG;
                ControlarUI(true);
                CargarSiguienteRonda();
            }
            if (modoJuego == 3)
            {
                ConfigurarUI_AdivinaArtista();
                currentGameId = gameG;
                ControlarUI(true);
                CargarSiguienteRonda();
            }
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
                            MessageBox.Show("No hay opciones disponibles en la base de datos");
                            FinalizarJuego();
                            return;
                        }

                        correctLyric = reader["Lyric_Text"]?.ToString() ?? string.Empty;
                        currentSongId = Convert.ToInt32(reader["Id_Song"]);
                        songFilePath = reader["File_Path"]?.ToString() ?? string.Empty;
                        titleSong = reader["Title"]?.ToString() ?? string.Empty;
                        artist = reader["Artist"]?.ToString() ?? string.Empty;
                        startTime = reader.GetDouble("Preview_Start");

                        if (modoJuego == 1)
                        {
                            if (string.IsNullOrEmpty(correctLyric))
                                lblLetraActual.Text = "Letra no disponible";
                            else
                                lblLetraActual.Text = $"🎤 Escucha y completa: {titleSong}";
                        }
                        if (modoJuego == 2)
                        {
                            if (string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(titleSong))
                                lblLetraActual.Text = "Artista o cancion no disponible";
                            else
                                lblLetraActual.Text = $"🎤 Escucha y adivina el artista y cancion";
                        }
                        if(modoJuego == 3)
                        {
                            if (string.IsNullOrEmpty(artist))
                                lblLetraActual.Text = "Artista no disponible";
                            else
                                lblLetraActual.Text = $"🎤 Escucha y adivina el artista de: {titleSong}";
                        }

                    }

                    // para el modo de juego 1:
                    if (modoJuego == 1)
                    {
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

                        for (int i = 0; i < 4; i++)
                        {
                            var rb = opcionesRadio[i];
                            rb.Text = opciones[i];
                            rb.Checked = false;
                            rb.Enabled = true;
                            rb.ForeColor = Color.FromArgb(50, 50, 50);
                        }
                    }
                    if (modoJuego == 2)
                    {
                        // 2. Distractores (3 artistas y canciones falsas)
                        cn.Close();
                        cn.Open();

                        query = @"
                SELECT Title, Artist 
                FROM songs
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
                                distractors.Add($"{r["Title"]?.ToString() ?? ""} - {r["Artist"]?.ToString() ?? ""}");
                            }
                        }

                        if (distractors.Count < 3)
                        {
                            MessageBox.Show("No hay suficientes artistas o canciones");
                            FinalizarJuego();
                            return;
                        }

                        // Mezclar opciones
                        string[] opciones = new string[4];
                        Random rnd = new Random();
                        int posCorrecta = rnd.Next(0, 4);
                        opciones[posCorrecta] = $"{titleSong} - {artist}";
                        int idx = 0;
                        for (int i = 0; i < 4; i++)
                            if (i != posCorrecta)
                                opciones[i] = distractors[idx++];
                        for (int i = 0; i < 4; i++)
                        {
                            var rb = opcionesRadio[i];
                            rb.Text = opciones[i];
                            rb.Checked = false;
                            rb.Enabled = true;
                            rb.ForeColor = Color.FromArgb(50, 50, 50);
                        }
                    }
                    if (modoJuego == 3)
                    {
                        // 2. Distractores (3 letras falsas)
                        cn.Close();
                        cn.Open();

                        query = @"
                SELECT Artist, Title
                FROM songs
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
                                    distractors.Add(r["Artist"]?.ToString() ?? "");
                            }
                        }

                        if (distractors.Count < 3)
                        {
                            MessageBox.Show("No hay suficientes artistas para distractores");
                            FinalizarJuego();
                            return;
                        }

                        // Mezclar opciones
                        string[] opciones = new string[4];
                        Random rnd = new Random();
                        int posCorrecta = rnd.Next(0, 4);
                        opciones[posCorrecta] = artist;

                        int idx = 0;
                        for (int i = 0; i < 4; i++)
                            if (i != posCorrecta)
                                opciones[i] = distractors[idx++];

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando ronda: " + ex.Message);
                FinalizarJuego();
            }

            ControlarUI(true);
            btnReproducir.Enabled = true;
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
                    .GetWithHighestBitrate();

                if (streamInfo == null)
                    throw new Exception("No se encontró stream de audio compatible");

                string audioUrl = streamInfo.Url;
                audioFile = new MediaFoundationReader(audioUrl);

                if (startTime > 0)
                    audioFile.CurrentTime = TimeSpan.FromSeconds(startTime);

                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Detener a los 6 segundos
                var timer = new System.Windows.Forms.Timer { Interval = 7000 };
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
            DetenerAudio();
            RadioButton? seleccionada = opcionesRadio?.FirstOrDefault(r => r.Checked);
            if (seleccionada == null)
            {
                MessageBox.Show("¡Selecciona una opción antes de verificar!");
                return;
            }

            // --- LÓGICA DE VERIFICACIÓN Y PUNTUACIÓN ---
            bool esCorrecta;
            string respuestaCorrecta;
            int puntosOtorgados = 0;

            if (modoJuego == 1)
            {
                respuestaCorrecta = correctLyric ?? string.Empty;
                esCorrecta = seleccionada.Text == respuestaCorrecta;
                puntosOtorgados = esCorrecta ? 10 : 0;
            }
            else // modoJuego == 2
            {
                // La respuesta correcta es la cadena combinada: "Título - Artista"
                respuestaCorrecta = $"{titleSong} - {artist}";
                esCorrecta = seleccionada.Text == respuestaCorrecta;
                puntosOtorgados = esCorrecta ? 10 : 0;
            }
            if (modoJuego == 3)
            {
                respuestaCorrecta = artist ?? string.Empty;
                esCorrecta = seleccionada.Text == respuestaCorrecta;
                puntosOtorgados = esCorrecta ? 10 : 0;
            }

                score += puntosOtorgados; // Actualizar el puntaje total

            // --- LÓGICA DE GUARDADO EN BASE DE DATOS ---
            try
            {
                using (MySqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();
                    // Usamos una lógica más limpia y consistente para el INSERT
                    string query = "INSERT INTO rounds (Id_Game, Id_Song, Guessed_title, Guessed_artist, Guessed_Lyric, Point_awarded) VALUES (@game, @song, @title, @artist, @lyric, @puntos)";
                    MySqlCommand cmd = new MySqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@game", currentGameId);
                    cmd.Parameters.AddWithValue("@song", currentSongId);
                    cmd.Parameters.AddWithValue("@puntos", puntosOtorgados);

                    if (modoJuego != 3)
                    {
                        if (modoJuego == 1)
                        {
                            // Se adivinó la letra: 1 si es correcta, 0 si no lo es.
                            cmd.Parameters.AddWithValue("@lyric", esCorrecta ? 1 : 0);
                            cmd.Parameters.AddWithValue("@title", 0);
                            cmd.Parameters.AddWithValue("@artist", 0);
                        }
                        else // modoJuego == 2
                        {
                            // Se adivinó título/artista (combinado): 1 si es correcta, 0 si no lo es.
                            cmd.Parameters.AddWithValue("@lyric", 0);
                            cmd.Parameters.AddWithValue("@title", esCorrecta ? 1 : 0);
                            cmd.Parameters.AddWithValue("@artist", esCorrecta ? 1 : 0);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@lyric", 0);
                        cmd.Parameters.AddWithValue("@title", 0);
                        cmd.Parameters.AddWithValue("@artist", esCorrecta ? 1 : 0);
                    }

                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar ronda: {ex.Message}");
            }

            // --- FEEDBACK VISUAL Y MENSAJES ---
            if (opcionesRadio != null)
            {
                foreach (RadioButton rb in opcionesRadio)
                {
                    if (rb == null) continue;
                    if (!rb.IsDisposed) rb.Enabled = false;

                    // Colorear en base a la respuesta Correcta o la selección Incorrecta
                    if (rb.Text == respuestaCorrecta)
                    {
                        if (!rb.IsDisposed) rb.ForeColor = Color.Green;
                    }
                    else if (rb.Checked && !esCorrecta)
                    {
                        if (!rb.IsDisposed) rb.ForeColor = Color.Red;
                    }
                }
            }

            if (lblScore != null && !lblScore.IsDisposed) lblScore.Text = $"Puntos: {score}";

            // Mensaje de feedback unificado
            if (esCorrecta)
            {
                MessageBox.Show($"🎉 ¡CORRECTO! +{puntosOtorgados} puntos");
            }
            else
            {
                string mensajeIncorrecto = modoJuego == 1
                    ? $"Incorrecto 😢.\nLa letra correcta era: \"{respuestaCorrecta}\""
                    : $"Incorrecto 😢.\nLa canción y artista correctos eran: \"{respuestaCorrecta}\"";

                MessageBox.Show(mensajeIncorrecto);
            }

            ControlarUI(false);
        }
        private void BtnSiguiente_Click(object? sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing) return;

            CargarSiguienteRonda();
        }
        private void FinalizarJuego()
        {     
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

using Plugin.Maui.Audio;
using System.Collections.ObjectModel;

namespace OdtwarzaczMuzyki
{
    public partial class CurrentlyPlaying : ContentPage
    {
        private readonly ObservableCollection<Song> _songs;
        private readonly IAudioManager _audioManager;
        private IAudioPlayer? _player;
        private int _currentSongIndex;
        private bool _isPlaying = false;
        private bool _like = false;
        private bool _sliderDragging = false;

        private bool _timerStarted = false;

        public CurrentlyPlaying(ObservableCollection<Song> songs, int startIndex = 0)
        {
            InitializeComponent();

            _songs = songs;
            _audioManager = AudioManager.Current;
            _currentSongIndex = startIndex;

            PlayCurrentSong();
        }

        private async void PlayCurrentSong()
        {
            try
            {
                var song = _songs[_currentSongIndex];
                BindingContext = song;

                // Usuń poprzednie eventy i player
                if (_player != null)
                {
                    _player.PlaybackEnded -= PlayerPlaybackEnded;
                    _player.Stop();
                    _player.Dispose();
                }

                // Tworzenie nowego playera
                var stream = File.OpenRead(song.Path!);
                _player = _audioManager.CreatePlayer(stream);

                _player.PlaybackEnded += PlayerPlaybackEnded;
                _player.Play();

                playBtn.Source = "pause.png";
                _isPlaying = true;

                // Uruchom timer tylko raz
                if (!_timerStarted)
                {
                    _timerStarted = true;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        if (_player == null)
                            return false;

                        if (!_sliderDragging)
                        {
                            songSlider.Value = _player.CurrentPosition;
                            UpdateTimeLabels();
                        }

                        return true; // kontynuuj timer
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie można odtworzyć piosenki: {ex.Message}", "OK");
            }
        }

        private void PlayerPlaybackEnded(object sender, EventArgs e)
        {
            NextSong();
        }

        private void playBtn_Clicked(object sender, EventArgs e)
        {
            if (_player == null)
                return;

            if (_isPlaying)
            {
                _player.Pause();
                playBtn.Source = "play.png";
                playBtn.Padding = new Thickness(10, 0, 5, 0);
                _isPlaying = false;
            }
            else
            {
                _player.Play();
                playBtn.Source = "pause.png";
                playBtn.Padding = new Thickness(5, 0, 5, 0);
                _isPlaying = true;
            }
        }

        private void prevBtn_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentSongIndex = (_currentSongIndex - 1 + _songs.Count) % _songs.Count;
            PlayCurrentSong();
        }

        private void nextBtn_Clicked(object sender, EventArgs e)
        {
            NextSong();
        }

        private void NextSong()
        {
            if (_songs.Count == 0) return;

            _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
            PlayCurrentSong();
        }

        private void likeBtn_Clicked(object sender, EventArgs e)
        {
            _like = !_like;
            likeBtn.Text = _like ? "❤" : "🤍";
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_player == null || !_sliderDragging)
                return;

            _player.Seek(e.NewValue);
            UpdateTimeLabels();
        }

        private void songSlider_DragStarted(object sender, EventArgs e)
        {
            _sliderDragging = true;
        }

        private void songSlider_DragCompleted(object sender, EventArgs e)
        {
            _sliderDragging = false;
            _player?.Seek(songSlider.Value);
        }

        private void UpdateTimeLabels()
        {
            if (_player == null)
                return;

            int position = (int)_player.CurrentPosition;
            int minutes = position / 60;
            int seconds = position % 60;
            timePlayed.Text = $"{minutes}:{seconds:D2}";

            if (BindingContext is Song song)
                songDurationLabel.Text = song.SongLengthFormatted;
        }
    }
}

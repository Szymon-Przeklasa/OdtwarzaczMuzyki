using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Media;
using System;

namespace OdtwarzaczMuzyki
{
    public partial class CurrentlyPlaying : ContentPage
    {
        public bool play = false;
        public bool like = false;
        private Song _song;

        public CurrentlyPlaying(Song song)
        {
            InitializeComponent();

            _song = song;
            BindingContext = _song;

            Player.Source = _song.Path;
            Player.Play();

            songSlider.Maximum = _song.SongLengthInSeconds;
        }

        private void likeBtn_Clicked(object sender, EventArgs e)
        {
            if (like == false)
            {
                like = true;
                likeBtn.Text = "❤";
            }
            else
            {
                like = false;
                likeBtn.Text = "🤍";
            }
        }

        private void playBtn_Clicked(object sender, EventArgs e)
        {
            
            /* int sliderValue = (int)songSlider.Value;
            string songdurationValue = songDurationLabel.Text;
            string[] songdParts = songdurationValue.Split(":");
            int minutes = Int32.Parse(songdParts[0]);
            int seconds = Int32.Parse(songdParts[1]);
            int songDuration = (minutes * 60) + seconds; */

            if (!play)
            {
                play = true;
                playBtn.Source = "pause.png";
                playBtn.Padding = new Thickness(5,2);

                //logika grania piosenki
                
            }
            else
            {
                play = false;
                playBtn.Source = "play.png";
                playBtn.Padding = new Thickness(10,0,5,0);

                //logika zatrzymywania piosenki
            }
        }


        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int sliderValue = (int)songSlider.Value;
            int minutes = sliderValue / 60;
            int seconds = sliderValue % 60;
            string sliderFormatted = $"{minutes}:{seconds:D2}";
            timePlayed.Text = sliderFormatted;

            if (play)
            {
                Player.SeekTo(TimeSpan.FromSeconds(sliderValue));
            }
        }

        private async void Player_CurrentStateChanged(object sender, EventArgs e)
        {
            if (Player.CurrentState == MediaElementState.Playing)
            {
                while (Player.CurrentState == MediaElementState.Playing)
                {
                    songSlider.Value = Player.Position.TotalSeconds;

                    int sliderValue = (int)songSlider.Value;
                    string sliderFormatted = $"{sliderValue / 60}:{sliderValue % 60:D2}";
                    timePlayed.Text = sliderFormatted;

                    await Task.Delay(1000);
                }
            }
        }

        private void prevBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void nextBtn_Clicked(object sender, EventArgs e)
        {

        }
    }
}

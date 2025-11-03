using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Views;

namespace OdtwarzaczMuzyki;

public partial class CurrentlyPlaying : ContentPage
{
    public bool like = false;
    public bool play = false;
	public CurrentlyPlaying()
	{
		InitializeComponent();

        var song = new Song
        {
            Cover = "dotnet_bot.png",
            SongName = "Imagine",
            Artist = "John Lennon",
            SongLengthInSeconds = 183
        };

        BindingContext = song;
    }
    private void likeBtn_Clicked(object sender, EventArgs e)
    {
        if(like == false)
        {
            like = true;
            likeBtn.Text = "❤";
        } else
        {
            like = false;
            likeBtn.Text = "🤍";
        }
    }

    private async void playBtn_Clicked(object sender, EventArgs e)
    {
        int sliderValue = (int)songSlider.Value;
        string songdurationValue = songDurationLabel.Text;
        string[] songdParts = songdurationValue.Split(":");
        int minutes = Int32.Parse(songdParts[0]);
        int seconds = Int32.Parse(songdParts[1]);
        int songDuration = (minutes * 60) + seconds;
        string songdFormatted = $"{songDuration / 60}:{songDuration % 60:D2}";
        while (play == false)
        {
            play = true;
            playBtn.Source = "pause.png";
            for(int i = sliderValue; i < songDuration; i++)
            {
                songSlider.Value = i;
                songDurationLabel.Text = songdFormatted;
                await Task.Delay(1000);
            }
        } 
        play = false;
        playBtn.Source = "play.png";

    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int sliderValue = (int) songSlider.Value;
        string sliderFormatted = $"{sliderValue / 60}:{sliderValue % 60:D2}";
        timePlayed.Text = sliderFormatted;
    }
}
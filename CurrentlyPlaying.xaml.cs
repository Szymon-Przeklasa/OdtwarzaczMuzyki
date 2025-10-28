namespace OdtwarzaczMuzyki;

public partial class CurrentlyPlaying : ContentPage
{
    public bool like = false;
	public CurrentlyPlaying()
	{
		InitializeComponent();

        var song = new Song
        {
            Cover = "dotnet_bot.png", // lub ImageSource.FromFile("dotnet_bot.png")
            SongName = "Imagine",
            Artist = "John Lennon",
            SongLengthInSeconds = 183
        };

        BindingContext = song;
    }

    private async void returnBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
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

    private void playBtn_Clicked(object sender, EventArgs e)
    {

    }
}
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace OdtwarzaczMuzyki
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Playlist> Playlists;
        private readonly string _playlistsFilePath;

        public MainPage()
        {
            InitializeComponent();
            _playlistsFilePath = Path.Combine(FileSystem.AppDataDirectory, "playlists.json");

            Playlists = new ObservableCollection<Playlist>();
            playlistsList.ItemsSource = Playlists;
            BindingContext = this;

            _ = LoadPlaylistsAsync();
        }

        private async void CreatePlaylistBtn_Clicked(object sender, EventArgs e)
        {
            string playlistName = await DisplayPromptAsync("Create a playlist", "Enter Playlist Name:");

            if (!string.IsNullOrEmpty(playlistName))
            {
                var newPlaylist = new Playlist
                {
                    Name = playlistName,
                    Songs = new ObservableCollection<Song>(),
                    SongCount = 0
                };

                Playlists.Add(newPlaylist);
                await SavePlaylistsAsync();
            }
        }

        private async void deletePlaylistBtn_Clicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var playlist = (Playlist) button.BindingContext;

            bool confirm = await DisplayAlert("Delete Playlist", $"Are you sure you want to delete \"{playlist.Name}\"?", "Yes", "No");
            if (confirm)
            {
                Playlists.Remove(playlist);

                var playlistFilePath = Path.Combine(FileSystem.AppDataDirectory, $"{playlist.Name}.json");
                if (File.Exists(playlistFilePath)) File.Delete(playlistFilePath);

                await SavePlaylistsAsync();
            }
        }
        private async Task SavePlaylistsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(Playlists);
                await File.WriteAllTextAsync(_playlistsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu playlist: {ex.Message}");
            }
        }
        private async Task LoadPlaylistsAsync()
        {
            try
            {
                if (File.Exists(_playlistsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_playlistsFilePath);
                    var loadedPlaylists = JsonSerializer.Deserialize<ObservableCollection<Playlist>>(json);

                    if (loadedPlaylists != null)
                    {
                        Playlists.Clear();
                        foreach (var playlist in loadedPlaylists) { Playlists.Add(playlist) };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu playlist: {ex.Message}");
            }
        }

        private async void playlistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Playlist selectedPlaylist)
            {
                await Navigation.PushAsync(new PlaylistPage(selectedPlaylist));

                ((CollectionView)sender).SelectedItem = null;
            }
        }

    }
}

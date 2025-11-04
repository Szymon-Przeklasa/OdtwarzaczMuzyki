using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace OdtwarzaczMuzyki
{
    public partial class PlaylistPage : ContentPage
    {
        private Playlist _playlist;
        private readonly string _playlistFilePath;

        public PlaylistPage(Playlist playlist)
        {
            InitializeComponent();

            _playlist = playlist;
            playlistNameLabel.Text = playlist.Name;
            songsList.ItemsSource = _playlist.Songs;

            _playlistFilePath = Path.Combine(FileSystem.AppDataDirectory, $"{playlist.Name}.json");

            _ = LoadSongsAsync();
        }
        private async void AddSong_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Wybierz pliki muzyczne",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".flac" } },
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.iOS, new[] { "public.audio" } }
                    })
                });

                if (result != null)
                {
                    foreach (var file in result)
                    {
                        string title = Path.GetFileNameWithoutExtension(file.FileName);
                        string artist = "Unknown";
                        
                        string path = file.FullPath;
                        var tfile = TagLib.File.Create(file.FullPath);
                        if(!string.IsNullOrEmpty(tfile.Tag.FirstPerformer))
                        {
                            artist = tfile.Tag.FirstPerformer;
                        }
                        double duration = tfile.Properties.Duration.TotalSeconds;
                        _playlist.Songs.Add(new Song
                            {
                                SongName = title,
                                Artist = artist,
                                SongLengthInSeconds = (int) duration,
                                Path = path
                        });
                    }
                    _playlist.SongCount = _playlist.Songs.Count;

                    await SaveSongsAsync();
                    await UpdateMainPlaylistsFileAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("B³¹d", $"Nie uda³o siê wybraæ plików: {ex.Message}", "OK");
            }
        }
        private async Task SaveSongsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_playlist.Songs);
                await File.WriteAllTextAsync(_playlistFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d zapisu: {ex.Message}");
            }
        }

        private async Task LoadSongsAsync()
        {
            try
            {
                if (File.Exists(_playlistFilePath))
                {
                    var json = await File.ReadAllTextAsync(_playlistFilePath);
                    var loadedSongs = JsonSerializer.Deserialize<ObservableCollection<Song>>(json);

                    if (loadedSongs != null)
                    {
                        _playlist.Songs.Clear();
                        foreach (var song in loadedSongs) { _playlist.Songs.Add(song); };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d odczytu: {ex.Message}");
            }
        }

        private async void DeleteSong_Clicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var song = (Song)button.BindingContext;
            if (song == null) return;

            bool confirm = await DisplayAlert("Delete Song", $"Are you sure you want to delete \"{song.SongName}\"?", "Yes", "No");

            if (confirm) {
                _playlist.Songs.Remove(song);
                _playlist.SongCount = _playlist.Songs.Count;
                await SaveSongsAsync();
                await UpdateMainPlaylistsFileAsync();
            }
        }

        private async Task UpdateMainPlaylistsFileAsync()
        {
            try
            {
                string playlistsPath = Path.Combine(FileSystem.AppDataDirectory, "playlists.json");

                if (!File.Exists(playlistsPath))
                    return;

                var json = await File.ReadAllTextAsync(playlistsPath);
                var playlists = JsonSerializer.Deserialize<ObservableCollection<Playlist>>(json) ?? new();

                var existing = playlists.FirstOrDefault(p => p.Name == _playlist.Name);
                if (existing != null)
                {
                    existing.SongCount = _playlist.Songs.Count;
                    existing.Songs = _playlist.Songs;
                }

                var updatedJson = JsonSerializer.Serialize(playlists);
                await File.WriteAllTextAsync(playlistsPath, updatedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d aktualizacji playlist.json: {ex.Message}");
            }
        }


        private async void Play_Clicked(object sender, EventArgs e)
        {
            if (_playlist.Songs.Count > 0)
            {
                var firstSong = _playlist.Songs[0];
                await Navigation.PushAsync(new CurrentlyPlaying(firstSong));
            }
            else
            {
                await DisplayAlert("Brak Piosenek", "W playliœcie nie ma ¿adnych piosenek do odtworzenia.", "OK");
            }
        }
    }

}

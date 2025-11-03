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

                    await SaveSongsAsync();
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
                        foreach (var song in loadedSongs) { _playlist.Songs.Add(song) };
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

            if (confirm) _playlist.Songs.Remove(song);
        }
    }

}

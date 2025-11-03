using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdtwarzaczMuzyki
{
    public class Playlist
    {
        public string Name { get; set; }
        public int SongCount { get; set; }
        public ObservableCollection<Song> Songs { get; set; } = new();
    }
}

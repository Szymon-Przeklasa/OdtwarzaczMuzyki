using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdtwarzaczMuzyki
{
    public class Song
    {
        public string? SongName { get; set; }
        public string? Artist { get; set; }
        public int SongLengthInSeconds { get; set; }
        public string SongLengthFormatted => $"{SongLengthInSeconds / 60}:{SongLengthInSeconds % 60:D2}";

        public string? Path { get; set; }
    }
}

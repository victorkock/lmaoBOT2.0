using System.Collections.Generic;
using Newtonsoft.Json;

namespace lmaoBOT
{
    public class Playlist
    {
        public string User { get; set; }
        public IList<string> Songs { get; set; }
    }
}
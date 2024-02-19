using MyLangs4Extensions.Models;

namespace MyLangs4Extensions.Models
{
    public class Extension
    {
        public string Name { get; set; }
        public string Pkg { get; set; }
        public string Apk { get; set; }
        public string Lang { get; set; }
        public int Code { get; set; }
        public string Version { get; set; }
        public int Nsfw { get; set; }
        public List<Source> Sources { get; set; }
    }
}
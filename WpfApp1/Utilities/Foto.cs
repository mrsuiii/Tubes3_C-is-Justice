using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utilities
{
    public class Foto
    {
        public string AsciiRepresentation { get; set; }
        public string Path { get; set; }
        public Foto() { 
        Path= string.Empty;
        AsciiRepresentation= string.Empty;
        
        }
        public Foto(string path, string asciiRepresentation)
        {
            Path = path;
            AsciiRepresentation = asciiRepresentation;
        }
        public string getAscii() { return AsciiRepresentation; }        
        public string getPath() { return Path; }
    }
}
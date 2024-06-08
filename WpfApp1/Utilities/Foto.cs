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

        public string Name { get; set; }
        public Foto() { 
            Path= string.Empty;
            AsciiRepresentation= string.Empty;
            Name = string.Empty;
        }
        public Foto(string path, string asciiRepresentation, string name)
        {
            Path = path;
            AsciiRepresentation = asciiRepresentation;
            Name = name;
        }
        public string getAscii() { return AsciiRepresentation; }        
        public string getPath() { return Path; }
        public string getName() { return Name; }

        public override string ToString()
        {
            return $"Nama: {Name}";
        }
    }
}
namespace ConsoleAsciiApp.Utilities
{
    public class Foto
    {
        public string AsciiRepresentation { get; set; }
        public string Path { get; set; }

        public Foto(string path, string asciiRepresentation)
        {
            Path = path;
            AsciiRepresentation = asciiRepresentation;
        }
    }
}

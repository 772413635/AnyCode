using System.IO;

namespace Common
{
    public class FileRead
    {
        public FileRead()
        {
            Fstream = null;
            Name = "";
            Path = "";
            Fstream = null;
            Process = 0;
            Length = 0;
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public Stream Fstream { get; set; }
        public long Process { get; set; }
        public long Length { get; set; }
    }
}

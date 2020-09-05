using System;

namespace FileServer.Database
{
    public class DbFileInfo
    {
        public string FileId { get; set; }
        public string ServerFileName { get; set; }
        public string FileName { get; set; }
        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }
    }
}

using System.IO;

namespace FileServer.StorageProvider
{
    public interface IStorageProvider
    {
        public void UploadFile(string fileName, Stream inputFileStream);

        public void DownloadFileAsync(string fileName, ProducerConsumerStream outFileStream);
    }
}

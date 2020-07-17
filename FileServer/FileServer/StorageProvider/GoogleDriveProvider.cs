using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileServer.StorageProvider
{
    public class GoogleDriveProvider : IStorageProvider
    {
        private readonly DriveService _service;
        private readonly string _driveFolder;
        public GoogleDriveProvider(IConfiguration configuration)
        {
            _driveFolder = configuration["GoogleDrive:DriveFolder"];
            var driveAccount = configuration["GoogleDrive:UserAccount"];
            var jsonCredPath = configuration["GoogleDrive:ServiceAccountJsonFile"];

            var text = File.ReadAllText(jsonCredPath);
            var credentials = GoogleCredential.FromJson(text);
            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(DriveService.Scope.Drive);
            }
            credentials = credentials.CreateWithUser(driveAccount);

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ".NET Core File Share",
            });
            _service = service;

        }

        public void DownloadFileAsync(string fileName, ProducerConsumerStream outStream)
        {
            DriveService driveService = _service;
            FilesResource.ListRequest listRequest = _service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q = $"'{_driveFolder}' in parents and name = '{fileName}'";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            string gFileId = files[0].Id;

            var request = driveService.Files.Get(gFileId);
            request.MediaDownloader.ProgressChanged += progress =>
            {
                if (progress.Status == DownloadStatus.Completed)
                {
                    outStream.CompleteAdding();
                }
            };
            request.DownloadAsync(outStream);

        }

        public void UploadFile(string fileName, Stream inputFileStream)
        {
            DriveService driveService = _service;
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new [] { _driveFolder }
            };

            FilesResource.CreateMediaUpload request;
            request = driveService.Files.Create(
                fileMetadata, inputFileStream, "application/octet-stream");
            request.Fields = "id";
            request.Upload();

        }
    }
}

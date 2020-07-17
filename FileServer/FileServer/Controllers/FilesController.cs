using System;
using System.Collections.Generic;
using FileServer.Crypto;
using FileServer.Database;
using FileServer.StorageProvider;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IStorageProvider _googleDriveInterface;
        private readonly IDatabase _databaseInterface;

        public FilesController(IStorageProvider googleDriveInterface, IDatabase databaseInterface)
        {
            _googleDriveInterface = googleDriveInterface;
            _databaseInterface = databaseInterface;
        }

        // GET: api/<FilesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FilesController>/5
        [HttpGet("{fileId}")]
        public IActionResult Get(string fileId)
        {
            Guid fileGuid;
            try
            {
                fileGuid = new Guid(fileId);
            }
            catch (FormatException)
            {
                return NotFound();
            }

            DbFileInfo fileInfo = _databaseInterface.GetFileInfo(fileGuid);
            if (fileInfo == null)
            {
                return NotFound();
            }
            var fileCrypto = new ChaChaFileCrypto()
            {
                Key = fileInfo.Key,
                Iv = fileInfo.Iv
            };

            var dloadStream = new ProducerConsumerStream(1048576);
            _googleDriveInterface.DownloadFileAsync(fileId, dloadStream);
            var outStream = fileCrypto.BcDecryptStream(dloadStream);

            return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                await outStream.CopyToAsync(outputStream);
                await outputStream.FlushAsync();
            })
            {
                FileDownloadName = fileInfo.FileName
            };

        }

        [HttpOptions]
        public IActionResult PreflightRoute()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "FileName, Content-Type");
            Response.Headers.Add("Access-Control-Allow-Method", "POST");
            return NoContent();
        }

        // POST api/<FilesController>
        [HttpPost]
        public IActionResult Post()
        {
            string fileName = Request.Headers["FileName"];

            Guid g = Guid.NewGuid();
            var fileCrypto = new ChaChaFileCrypto();
            var cs = fileCrypto.BcEncryptStream(Request.Body);

            _googleDriveInterface.UploadFile(g.ToString(), cs);
            var fileInfo = new DbFileInfo
            {
                FileId = g,
                FileName = fileName,
                Key = fileCrypto.Key,
                Iv = fileCrypto.Iv
            };
            _databaseInterface.StoreFileDetails(fileInfo);

            Dictionary<string, string> responseDict = new Dictionary<string, string>
            {
                ["fileId"] = g.ToString()
            };
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return Ok(responseDict);

        }

        // PUT api/<FilesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FilesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContentApp.Data;
using ContentApp.FileStorage;
using ContentApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ContentAppDbContext dbContext;
        private readonly IFileStoragePiece fileStoragePiece;
        private readonly ILogger<FilesController> logger;

        public FilesController(ContentAppDbContext dbContext,
            IFileStoragePiece fileStoragePiece,
            ILogger<FilesController> logger)
        {
            this.dbContext = dbContext;
            this.fileStoragePiece = fileStoragePiece;
            this.logger = logger;
        }

        // GET: api/Files
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            return new string[] { "foo", "foo" };
        }

        // GET: api/Files/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Files
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Files/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpPost("files")]
        public async Task<IActionResult> Images(List<IFormFile> files)
        {
            long size = 0;
            try
            {
                size = files.Sum(f => f.Length);

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var readStream = formFile.OpenReadStream())
                        {
                            logger.LogInformation($"Storing image file of type {formFile.ContentType} uploaded as file named {formFile.FileName}");

                            // IFileStoragePiece will read the stream as a jpeg image
                            // convert it to three different sizes and store each converted
                            // file in the blob store. The returned string is the prefix of
                            // all the files created.
                            var fileName = await this.fileStoragePiece.StoreImage(readStream);

                            // Create a record in the database to keep track of this uploaed file
                            var asset = new Asset
                            {
                                AssetType = AssetType.Photo,
                                FilePrefix = fileName,
                                CreatedOnUtc = DateTime.UtcNow,
                                UploadedFileName = formFile.FileName
                            };
                            this.dbContext.Assets.Add(asset);
                            await this.dbContext.SaveChangesAsync();

                            logger.LogInformation($"Completed storing image with file prefix {fileName}");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Error storing file");
            }

            return Ok(new { count = files.Count, size });
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

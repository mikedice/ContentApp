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

namespace ContentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ContentAppDbContext dbContext;
        private readonly IFileStoragePiece fileStoragePiece;
        private readonly IConfiguration configuration;
        public FilesController(ContentAppDbContext dbContext,
            IFileStoragePiece fileStoragePiece,
            IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.fileStoragePiece = fileStoragePiece;
            this.configuration = configuration;
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
                        var filePath = Path.GetTempFileName();

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            using (var memStream = new MemoryStream())
                            {
                                await formFile.CopyToAsync(memStream);
                                memStream.Seek(0, SeekOrigin.Begin);
                                var fileName = await this.fileStoragePiece.StoreImage(memStream);

                                var asset = new Asset
                                {
                                    AssetType = AssetType.Photo,
                                    FilePrefix = fileName,
                                    CreatedOn = DateTime.UtcNow
                                };
                                this.dbContext.Assets.Add(asset);
                                await this.dbContext.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

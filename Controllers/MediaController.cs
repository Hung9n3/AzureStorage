using AzureStorage.Core.Database;
using AzureStorage.Core.Entities;
using AzureStorage.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureStorage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MediaController : ControllerBase
    {

        private readonly ILogger<MediaController> _logger;
        private Context _context;
        private AzureStorageSevice _storageService;
        public MediaController(ILogger<MediaController> logger, Context context, AzureStorageSevice storageService)
        {
            _logger = logger;
            this._context = context;
            this._storageService = storageService;
        }

        [HttpGet(Name = "GetContainer")]
        public async Task<IActionResult> GetContainer()
        {
            return Ok(await _context.BlobContainers.AsNoTracking().ToListAsync());
        }
        [HttpPost(Name ="AddContainer")]
        public async Task<IActionResult> AddContainer(string name)
        {
            var container = await _storageService.AddContainers(name);
            BlobContainer c = new BlobContainer() { Name = name };
            await _context.AddAsync<BlobContainer>(c);
            await _context.SaveChangesAsync();
            return Ok(await _context.BlobContainers.AsNoTracking().ToListAsync());
        }
        [HttpPost("{containerId}", Name = "UploadItem")]
        public async Task<IActionResult> UploadItem(IFormFile file, int containerId)
        {
            var container = await _context.BlobContainers.FindAsync(containerId);

            //Stream stream = file.OpenReadStream();
            Tuple<string, string> result = await _storageService.Upload(file, container!.Name);
            var blobName = result.Item1;
            var stringUrl = result.Item2;
            if (!String.IsNullOrEmpty(blobName) && !String.IsNullOrEmpty(stringUrl))
            {
                BlobItem newItem = new BlobItem()
                {
                    Name = blobName,
                    Url = stringUrl,
                    Container = containerId

                };
                _context.BlobItems.Add(newItem);
                await _context.SaveChangesAsync();
                return Ok(newItem);
            }
            else return BadRequest("Something went wrong");
            //return Ok(file.ContentType);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {

             var item = await _context.BlobItems.FindAsync(id);
            var container = await _context.BlobContainers.FindAsync(item.Container);
            await _storageService.Delete(item.Name, container.Name );
             _context.BlobItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
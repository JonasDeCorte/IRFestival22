using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        public BlobUtility BlobUtility { get; }

        public PicturesController(BlobUtility blobUtility)
        {
            BlobUtility = blobUtility;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public async Task<ActionResult> GetAllPictureUrls()
        {
            var container = BlobUtility.GetPicturesContainer();
            return Ok(container.GetBlobs().Select(blob => BlobUtility.GetSasUri(container, blob.Name)).ToArray());
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = BlobUtility.GetPicturesContainer();
            var name = file.FileName.Replace(" ", "");
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(name)}";
            await container.UploadBlobAsync(filename, file.OpenReadStream());

            return Ok();
        }
    }
}
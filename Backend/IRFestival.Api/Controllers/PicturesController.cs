using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Net;
using System.Text.Json;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        public BlobUtility BlobUtility { get; }
        private readonly IConfiguration Configuration;
        private readonly string[] ScopesRequiredByApiToUploadPictures = new string[] { "Pictures.Upload.All" };

        public PicturesController(BlobUtility blobUtility, IConfiguration configuration)
        {
            BlobUtility = blobUtility;
            Configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public async Task<ActionResult> GetAllPictureUrls()
        {
            var container = BlobUtility.GetThumbsContainer();
            return Ok(container.GetBlobs().Select(blob => BlobUtility.GetSasUri(container, blob.Name)).ToArray());
        }

        [HttpPost("upload")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(ScopesRequiredByApiToUploadPictures);
            BlobContainerClient container = BlobUtility.GetPicturesContainer();
            var fileNameWithoutSpecialChars = file.FileName.Replace(" ", "");
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(fileNameWithoutSpecialChars)}";
            await container.UploadBlobAsync(filename, file.OpenReadStream());
            await using (var client = new ServiceBusClient(Configuration.GetConnectionString("ServiceBusSenderConnection")))
            {
                ServiceBusSender sender = client.CreateSender(Configuration.GetValue<string>("QueueNameMails"));
                var obj = new
                {
                    Message = $"The picture {filename} was uploaded!",
                    Email = " me@you.us"
                };

                var json = JsonSerializer.Serialize(obj);
                var binary = BinaryData.FromString(json);

                ServiceBusMessage message = new ServiceBusMessage();
                message.Body = binary;
                await sender.SendMessageAsync(message);
            }
            return Ok();
        }
    }
}
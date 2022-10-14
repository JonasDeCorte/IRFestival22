using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace IRFestival.Function2
{
    public static class ThumbnailFunction
    {
        [FunctionName("ThumbnailFunction")]
        public static void Run(
            [BlobTrigger("festivalpics-uploaded/{name}", Connection = "BlobStorageConnection")] Stream image, string name, ILogger log,
             [Blob("festivalthumbs/{name}", FileAccess.Write, Connection = "BlobStorageConnection")] Stream thumbnail)
        {
            // test for rerun
            using Image<Rgba32> input = Image.Load<Rgba32>(image, out IImageFormat format);
            input.Mutate(i =>
            {
                i.Resize(340, 0);
                int height = i.GetCurrentSize().Height;
                i.Crop(new Rectangle(0, 0, 340, height < 226 ? height : 226));
            });
            input.Save(thumbnail, format);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {image.Length} Bytes");
        }
    }
}
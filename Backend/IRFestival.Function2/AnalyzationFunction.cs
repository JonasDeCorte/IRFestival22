using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IRFestival.Function2
{
    public class AnalyzationFunction
    {
        private static readonly List<VisualFeatureTypes?> Features = new()
        {
            VisualFeatureTypes.Adult
        };

        public AnalyzationFunction(ComputerVisionClient computerVisionClient)
        {
            ComputerVisionClient = computerVisionClient;
        }

        public ComputerVisionClient ComputerVisionClient { get; }

        [FunctionName("AnalyzationFunction")]
        public async Task Run([BlobTrigger("festivalpics-uploaded/{name}", Connection = "BlobStorageConnection")] byte[] myBlob, string name, ILogger log, Binder binder)
        {
            ImageAnalysis imageAnalysis = await ComputerVisionClient.AnalyzeImageInStreamAsync(new MemoryStream(myBlob), Features);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            Attribute[] attributes;
            if (imageAnalysis.Adult.IsAdultContent || imageAnalysis.Adult.IsGoryContent || imageAnalysis.Adult.IsRacyContent)
            {
                attributes = new Attribute[]
                {
                    new BlobAttribute($"festivalpics-rejected/{name}", FileAccess.Write),
                    new StorageAccountAttribute("BlobStorageConnection")
                };
            }
            else
            {
                attributes = new Attribute[]
                {
                    new BlobAttribute($"festivalpics-approved/{name}", FileAccess.Write),
                    new StorageAccountAttribute("BlobStorageConnection")
                };
            }
            using Stream fileoutputStream = await binder.BindAsync<Stream>(attributes);
            fileoutputStream.Write(myBlob);
        }
    }
}
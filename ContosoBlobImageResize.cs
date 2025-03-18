using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Bmp;

namespace MyFunctionApp {
    public class ResizeImageFunction
    {
        private readonly ILogger<ResizeImageFunction> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ResizeImageFunction(ILogger<ResizeImageFunction> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        [Function("ResizeImageFunction")]
        public async Task RunAsync(
            [BlobTrigger("", Connection = "")] Stream inputStream, 
            string name)
        {

            // Copy the incoming stream to a MemoryStream


            // Detect image format using ImageSharp


            // Resize the image to 100x100 pixels using Pad mode


            // Generate new file name with _thumb suffix
            

            // Choose the appropriate encoder based on file extension


            // Save the resized image into an output MemoryStream


            // Use the injected BlobServiceClient to get the "resized-product-images" container


            // Delete any existing blob to simulate "overwrite" behavior.
            

            // Create BlobUploadOptions to include HTTP headers
            

            // Upload the processed image
        

        }
    }
}
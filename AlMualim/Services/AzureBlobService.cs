using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs.Models;

namespace AlMualim.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly string ConnectionString;
        private readonly BlobContainerClient NotesContainer;
        private Random random = new Random();

        private const string CONTAINER_NAME_NOTES = "notes";

        public AzureBlobService(IConfiguration configuration)
        {
            // Get connection string
            ConnectionString = configuration.GetConnectionString("AzureBlobConnectionString");

            // Get Notes Container
            NotesContainer = new BlobContainerClient(ConnectionString, CONTAINER_NAME_NOTES);
        }

        public async Task<bool> IsBlobUrlReachable(string url)
        {
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest) WebRequest.Create(url);
            }
            catch(UriFormatException)
            {
                return false;
            }

            request.Timeout = 15000;
            request.Method = "GET";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        public async Task<string> UploadNotesAndGetURL(IFormFile notesFile)
        {
            var fileName = notesFile.FileName;
            var blobName = await GenerateBlobName(fileName);

            // Generate blob
            var blob = NotesContainer.GetBlobClient(blobName);
            
            // Upload file
            using(var stream = notesFile.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }

            // Get URL 
            var blobUrl = blob.Uri.AbsoluteUri;
            return blobUrl;
        }

        public async Task<string> UpdateExistingBlob(IFormFile notesFile, string blobUrl)
        {
            // If no URL is provided, treat as brand new
            if (blobUrl == null)
                return await UploadNotesAndGetURL(notesFile);

            // If provided, try to get the blob
            var blob = GetBlobFromUrl(blobUrl);

            // If it does exist, update the file
            using (var stream = notesFile.OpenReadStream())
                await blob.UploadAsync(stream, true);
            return blob.Uri.AbsoluteUri;
        }

        public async Task DeleteBlob(string url)
        {
            var blob = GetBlobFromUrl(url);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task DeleteAllExtraData(List<string> urls)
        {
            var blobNames = urls.Select(u => GetBlobNameFromUrl(u)).ToHashSet();
            // Run through all blobs
            await foreach(var blob in NotesContainer.GetBlobsAsync())
            {
                if (!blobNames.Contains(blob.Name))
                {
                    var blobClient = NotesContainer.GetBlobClient(blob.Name);
                    await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
                }
            }
        }
        
        #region Private Methods
        private async Task<string> GenerateBlobName(string inputString)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.".ToLower();
            var fileName = new String(inputString.ToLower().Replace(" ", "_").Where(c => chars.Contains(c)).ToArray());
            var blobName = fileName;
            var counter = 0;

            while (await NotesContainer.GetBlobClient(blobName).ExistsAsync() == true)
            {
                counter++;
                blobName = fileName.Replace(".pdf", "") + counter + ".pdf";
            }

            return blobName;
        } 

        private BlobClient GetBlobFromUrl(string url)
        {
            var name = url.Replace(NotesContainer.Uri.AbsoluteUri + "/", "");
            return NotesContainer.GetBlobClient(name);
        }

        private string GetBlobNameFromUrl(string url)
        {
            return url.Replace(NotesContainer.Uri.AbsoluteUri + "/", "");
        }
        #endregion
    }
}
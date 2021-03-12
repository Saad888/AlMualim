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
    public interface IAzureBlobService 
    {
        Task<bool> IsBlobUrlReachable(string url);

        Task<string> UploadNotesAndGetURL(IFormFile notesFile);

        Task<string> UpdateExistingBlob(IFormFile notesFile, string blobUrl);

        Task DeleteBlob(string url);

        Task DeleteAllExtraData(List<string> urls);
    }
}

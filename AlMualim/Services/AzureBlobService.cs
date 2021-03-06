using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Azure.Storage.Blobs;

namespace AlMualim.Services
{
    public static class AzureBlobService
    {
        private static readonly string ConnectionString;
        private static readonly BlobContainerClient NotesContainer;

        private const string CONTAINER_NAME_NOTES = "notes";

        static AzureBlobService()
        {
            // Build configuration service
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            // Get connection string
            ConnectionString = configuration.GetConnectionString("AzureBlobConnectionString");

            // Get Notes Container
            NotesContainer = new BlobContainerClient(ConnectionString, CONTAINER_NAME_NOTES);
        }

        public async static Task<bool> IsBlobUrlReachable(string url)
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

    }
}
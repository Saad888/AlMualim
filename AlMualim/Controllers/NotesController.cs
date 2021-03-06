using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Models;
using Azure.Storage.Blobs;

namespace AlMualim.Controllers
{
    public class NotesController : Controller
    {

        private const string CONNECTIONSTRING = "DefaultEndpointsProtocol=https;AccountName=almualum;AccountKey=+uhxaiFox090mr4OAz3STpzpZy3Z1cXCCWVrqtcTkUfejnwDkzZOL2LQO5lA+MpY+bJhDODViTlpAqEIH4IdPA==;EndpointSuffix=core.windows.net";

        private readonly AlMualimDbContext _context;

        public NotesController(AlMualimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
                return NotFound();

            var containerName = "notes";
            BlobContainerClient container = new BlobContainerClient(CONNECTIONSTRING, containerName);
            BlobClient blob = container.GetBlobClient("pdftest.pdf");
            var target = "THISHASBEENDOWNLOADED.pdf";
            blob.DownloadTo(target);



            return View();
        }
    }
}
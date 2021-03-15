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
using Microsoft.Extensions.Configuration;
using AlMualim.Services;

namespace AlMualim.Controllers
{
    public class NotesController : Controller
    {

        private readonly AlMualimDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAzureBlobService _azureBlobService;

        public NotesController(AlMualimDbContext context, IConfiguration config, IAzureBlobService azureBlobService)
        {
            _context = context;
            _configuration = config;
            _azureBlobService = azureBlobService;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
                return NotFound();

            // Get Notes 
            var note = await _context.Notes.Include(n => n.Topics).FirstOrDefaultAsync(n => n.ID == id);

            // If no notes found, return 404
            if (note == null)
                return NotFound();

            // Get topics and surah
            if (note.Surah != null) ViewData["Surah"] = Surah.AllSurah[(int)note.Surah];

            // Validate URL 
            var isUrlValid = await _azureBlobService.IsBlobUrlReachable(note.URL);
            ViewData["IsUrlValid"] = isUrlValid;

            // Store data
            return View(note);
        }
    }
}
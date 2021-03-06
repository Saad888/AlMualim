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

        public NotesController(AlMualimDbContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
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
            ViewData["Surah"] = await _context.Surah.FirstOrDefaultAsync(s => s.ID == note.Surah);

            // Store data
            return View(note);
        }
    }
}
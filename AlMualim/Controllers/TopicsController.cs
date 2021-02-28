using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Models;

namespace AlMualim.Controllers
{
    public class TopicsController : Controller
    {
        private readonly AlMualimDbContext _context;

        public TopicsController(AlMualimDbContext context)
        {
            _context = context;
        }

        // GET: Topics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Topics.ToListAsync());
        }

        // GET: Topics/Notes/ID
        public async Task<IActionResult> Notes(int id)
        {
            // Get notes list
            var notes = await _context.Notes.Where(n => n.Topics.Any(t => t.ID == id)).OrderBy(n => n.Surah).ToListAsync();

            // Get topic metadata
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.ID == id);
            ViewData["Topic"] = topic;

            // Get surah metadata
            var surah = await _context.Surah.ToListAsync();
            ViewData["Surah"] = surah;

            // Filter notes by 
            return View(notes);
        }
    }
}

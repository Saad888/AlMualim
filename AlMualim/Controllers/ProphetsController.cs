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
    public class ProphetsController : Controller
    {

        private readonly AlMualimDbContext _context;

        public ProphetsController(AlMualimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stories = await _context.Stories.Include(s => s.Notes).Where(s => s.Notes.Count() > 0).OrderByDescending(s => s.Order).ToListAsync();

            return View(stories);
        }

        public async Task<IActionResult> Notes(int? id)
        {
            if (id == null)
                return NotFound();

            var story = await _context.Stories.FirstOrDefaultAsync(s => s.ID == id);
            if (story == null)
                return NotFound();

            var notes = await _context.Notes.Where(n => n.Story.ID == id).OrderByDescending(n => n.StoryOrder).ToListAsync();
            ViewData["Surah"] = Surah.List();
            ViewData["Story"] = story;
            return View(notes);
        }
    }
}
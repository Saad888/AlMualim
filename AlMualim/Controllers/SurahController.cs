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

namespace AlMualim.Controllers
{
    public class SurahController : Controller
    {
        private readonly AlMualimDbContext _context;

        public SurahController(AlMualimDbContext context)
        {
            _context = context;
        }

        // GET: Surah/
        public async Task<IActionResult> Index()
        {
            // Get surah and notes list
            var surahList = await _context.Surah.ToListAsync();
            var notesList = await _context.Notes.ToListAsync();
            
            // Get list of surahs IDs that have notes
            var surahNotesIDs = (from note in notesList
                                select note.Surah).ToHashSet();
            var filteredSurahs = surahList.Where(s => surahNotesIDs.Contains(s.ID));            

            return View(filteredSurahs);
        }

        // GET: Surah/Notes/{slug}
        public async Task<IActionResult> Notes(string slug)
        {
            // if Slug is empty, return to Index
            if (String.IsNullOrEmpty(slug))
                return RedirectToAction("Index");

            // Get surah 
            var surah = await _context.Surah.FirstOrDefaultAsync(s => s.Slug == slug);
            if (surah == null)
                return NotFound();

            // Get notes list
            var notesList = _context.Notes.Where(n => n.Surah == surah.ID);

            // Get Ruku List
            var rukuList = (from note in notesList
                           orderby note.Ruku
                           select note.Ruku).Distinct().ToList();
            ViewData["RukuList"] = rukuList;

            // Get Surah Data
            ViewData["Surah"] = surah;

            // Render view
            return View(notesList);
        }
    }
}

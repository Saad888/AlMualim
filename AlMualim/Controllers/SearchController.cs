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
    public class SearchController : Controller
    {
        private readonly AlMualimDbContext _context;

        public SearchController(AlMualimDbContext context)
        {
            _context = context;
        }

        //GET: /Search?surah=id?&ruku=id?&topic=id?&search=string
        public async Task<IActionResult> Index(int? surah, int? ruku, int? topic, string search)
        {
            // If surah is not set as a parameter, ruku is also not a valid parameter
            if (surah == null) ruku = null;

            // If no search parameters, return view without results
            if ((surah == null) && (ruku == null) && (topic == null) && (String.IsNullOrEmpty(search)))
                return View();

            // If search results, get list of notes and filter
            var notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).ToListAsync();
            
            // Filter surah and ruku
            notes = notes.Where(n => ((surah != null ? n.Surah == surah : true) && (ruku != null ? n.Ruku == ruku : true))).ToList();

            // Filter topic
            notes = notes.Where(n => (topic != null ? n.Topics.Any(t => t.ID == topic) : true)).ToList();

            // Filter by search string
            if (String.IsNullOrEmpty(search))
            {
                var surahs = await _context.Surah.ToDictionaryAsync(s => s.ID); 
                notes = notes.Where(note => 
                {
                    var surah = note.Surah != null ? surahs[(int)note.Surah] : null;
                    var searchString = note.GetSearchString(surah);

                    return searchString.Contains(search.ToLower());
                }).ToList();
            }

            return View(notes);
        }
    }
}

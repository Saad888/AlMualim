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
using FuzzySharp;

namespace AlMualim.Controllers
{
    public class SearchController : Controller
    {
        private readonly AlMualimDbContext _context;
        private const int WEIGHT_LIMIT = 75;

        public SearchController(AlMualimDbContext context)
        {
            _context = context;
        }

        //POST: /Search
        public async Task<IActionResult> Index(int? surah, int? ruku, int? topic, string searchString)
        {
            // Get Surah and Topics list
            var surahs = await _context.Surah.ToListAsync(); 
            var topics = await _context.Topics.ToListAsync();
            ViewData["Surah"] = surahs;
            ViewData["Topics"] = topics;

            // Store search parameters
            var searchParams = new Dictionary<string, object>();
            searchParams.Add("Surah", surah);
            searchParams.Add("Ruku", ruku);
            searchParams.Add("Topic", topic);
            searchParams.Add("Search", searchString);
            ViewData["SearchParams"] = searchParams;

            // If surah is not set as a parameter, ruku is also not a valid parameter
            if (surah == null) ruku = null;

            // If no search parameters, return view without results
            if ((surah == null) && (ruku == null) && (topic == null) && (String.IsNullOrEmpty(searchString)))
                return View();

            // If search results, get list of notes and filter
            var notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).ToListAsync();
            
            // Filter surah and ruku
            notes = notes.Where(n => ((surah != null ? n.Surah == surah : true) && (ruku != null ? n.Ruku == ruku : true))).ToList();

            // Filter topic
            notes = notes.Where(n => (topic != null ? n.Topics.Any(t => t.ID == topic) : true)).ToList();

            // Filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                var weightedNotesList = notes.Select(note => 
                {
                    var surah = note.Surah != null ? surahs.FirstOrDefault(s => s.ID == note.Surah) : null;
                    var noteSearchString = note.GetSearchString(surah);
                    var ratio = Fuzz.PartialRatio(searchString.ToLower(), noteSearchString);
                    return new WeightedNotes() {Note = note, SearchWeight = ratio};
                }).Where(w => w.SearchWeight >= WEIGHT_LIMIT).OrderByDescending(w => w.SearchWeight);

                notes = weightedNotesList.Select(w => w.Note).ToList();
            }

            return View(notes);
        }
    }
}

public class WeightedNotes
{
    public Notes Note {get; set;}
    public int SearchWeight {get; set;}
}
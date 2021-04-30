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
            var surahsList = await _context.Notes.Select(n => n.Surah).ToListAsync();
            var surahs = Surah.List().Where(s => surahsList.Contains(s.ID)).ToList(); 
            var topics = await _context.Topics.OrderBy(t => t.Order).ToListAsync();
            var stories = await _context.Stories.ToListAsync();
            ViewData["Surah"] = surahs;
            ViewData["Topics"] = topics;
            ViewData["Stories"] = stories;

            return View();
        }

        public async Task<IActionResult> SearchResults(int? surah, int? ruku, int? topic, int? story, string searchString)
        {
            var surahs = Surah.List(); 
            ViewData["Surah"] = surahs;

            if (surah <= 0) surah = null;
            if (ruku <= 0) ruku = null;
            if (topic < 0 && topic != -2) topic = null; // -2 represents Islamic History
            if (story < 0) story = null;

            // If surah is not set as a parameter, ruku is also not a valid parameter
            if (surah == null) ruku = null;

            // If no search parameters, return view without results
            if ((surah == null) && (ruku == null) && (topic == null) && (story == null) && (String.IsNullOrEmpty(searchString)))
                return View();

            // If search results, get list of notes and filter
            var notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).Include(n => n.Story).ToListAsync();
            
            // Filter surah and ruku
            notes = notes.Where(n => ((surah != null ? n.Surah == surah : true) && (ruku != null ? n.Ruku == ruku : true))).ToList();

            // Filter topic
            if (topic == -2) notes = notes.Where(n => n.IsHistory).ToList();
            else notes = notes.Where(n => (topic != null ? n.Topics.Any(t => t.ID == topic) : true)).ToList();

            // Filter story
            if (story != null) notes = notes.Where(n => n.Story != null ? n.Story.ID == story : false).ToList();

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
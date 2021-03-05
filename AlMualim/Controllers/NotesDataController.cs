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
    public class NotesDataController : Controller
    {
        private readonly AlMualimDbContext _context;

        public NotesDataController(AlMualimDbContext context)
        {
            _context = context;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notes.Include(n => n.Topics).ToListAsync());
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allNotes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).ToListAsync();
            var notes = allNotes.FirstOrDefault(n => n.ID == id);

            if (notes == null)
            {
                return NotFound();
            }

            ViewData["AllTopics"] = _context.Topics.ToList().Select(i => i.Title);

            return View(notes);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            var topicsList = _context.Topics.ToList(); 
            ViewData["Topics"] = topicsList;
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Description,Surah,Ruku,Url,DateAdded,LastUpdated")] Notes notes, string TagInputs)
        {
            // Update times
            notes.DateAdded = DateTime.Now;
            notes.LastUpdated = DateTime.Now;

            // Get Topics List
            if (notes.Topics == null) notes.Topics = new List<Topics>();
            notes.Topics.Clear();
            var fullTopicsList = _context.Topics.ToList();
            var selectedTopics = fullTopicsList.Where(t => Request.Form["SelectedTopics"].Contains(t.ID.ToString())).ToList();
            selectedTopics.ForEach(t => notes.Topics.Add(t));

            // Get and save tags
            if(TagInputs != null)
            {
                if (notes.Tags == null) notes.Tags = new List<Tags>();
                var allTags = await _context.Tags.ToListAsync();
                var addedTags = TagInputs.ToLower().Split(" ").Distinct();
                foreach(var newTag in addedTags)
                {
                    var existingTag = allTags.FirstOrDefault(t => t.Title == newTag);
                    if (existingTag != null)
                    {
                        notes.Tags.Add(existingTag);
                    } 
                    else
                    {
                        var newTagObj = new Tags() {Title = newTag};
                        _context.Tags.Add(newTagObj);
                        notes.Tags.Add(newTagObj);
                    }
                }
            }

            // Submit if model state is valid
            if (ModelState.IsValid)
            {
                _context.Add(notes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notes);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allNotes = await _context.Notes.Include(n => n.Topics).ToListAsync();
            var notes = allNotes.FirstOrDefault(n => n.ID == id);
            if (notes == null)
            {
                return NotFound();
            }
            
            var topicsList = _context.Topics.ToList(); 
            ViewData["Topics"] = topicsList;
            return View(notes);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Description,Surah,Ruku,Url,DateAdded,LastUpdated,Topics")] Notes notes)
        {
            if (id != notes.ID)
            {
                return NotFound();
            }
            
            // Update times
            notes.LastUpdated = DateTime.Now;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotesExists(notes.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                // Update topics
                var originalNote = await _context.Notes.Include(c => c.Topics).FirstOrDefaultAsync(c => c.ID == notes.ID);
                originalNote.Topics.Clear();

                var originalTopics = await _context.Topics.ToListAsync();
                Request.Form["SelectedTopics"].ToList().ForEach(st => originalNote.Topics.Add(originalTopics.FirstOrDefault(ot => ot.ID.ToString() == st)));
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(notes);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notes = await _context.Notes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (notes == null)
            {
                return NotFound();
            }

            return View(notes);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notes = await _context.Notes.FindAsync(id);
            _context.Notes.Remove(notes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotesExists(int id)
        {
            return _context.Notes.Any(e => e.ID == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Models;
using Microsoft.Extensions.Configuration;
using AlMualim.Services;
using FuzzySharp;

namespace AlMualim.Controllers
{
    public class AdminController : Controller
    {
        private readonly AlMualimDbContext _context;
        private const int WEIGHT_LIMIT = 75;

        public AdminController(AlMualimDbContext context)
        {
            _context = context;
        }

        #region Index
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
        #endregion

        #region Add Notes
        public async Task<IActionResult> Add()
        {
            ViewData["Surah"] = await _context.Surah.ToListAsync();
            ViewData["Topics"] = await _context.Topics.ToListAsync();
            ViewData["ViewMode"] = "Add";
            return View("AddEdit");
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitType, 
                                             ICollection<int> selectedTopics, 
                                             IFormFile notesFile,
                                             string tags,
                                             [Bind("ID,Title,Description,Surah,Ruku,URL,DateAdded,LastUpdated")] Notes notes, 
                                             [Bind("AddTopicText, DeleteTopicId, EditTopicId, EditTopicText")] TopicModification topicMod)
        {
            // Verify action type
            if(submitType != "Add Notes" && submitType != "Edit Notes")
            {
                var newId = await ModifyTopics(submitType, topicMod);
                if (newId != null && submitType == "Add Topic")
                    selectedTopics.Add((int)newId);
            }

            var surah = await _context.Surah.ToListAsync();
            var topics = await _context.Topics.ToListAsync();

            // Update topics
            notes.Topics = topics.Where(t => selectedTopics.Contains(t.ID)).ToList();

            // if save, save
            if (submitType == "Add Notes")
            {
                // Update save and update dates
                notes.DateAdded = DateTime.Now;
                notes.LastUpdated = notes.DateAdded;

                // Update tags
                notes = await UpdateTags(notes, tags);

                // Upload and get URL
                if(notesFile != null)
                    notes.URL = await AzureBlobService.UploadNotesAndGetURL(notesFile);

                // Submit if model state is valid
                _context.Add(notes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } 

            ViewData["Surah"] = surah;
            ViewData["Topics"] = topics.OrderBy(t => t.Title).ToList();
            ViewData["Tags"] = tags;
            ViewData["ViewMode"] = "Add";
            return View("AddEdit", notes);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).FirstOrDefaultAsync(n => n.ID == id);
            if (notes == null)
                return NotFound();

            var tagsList = "";
            if (notes.Tags != null)
            {
                tagsList = String.Join(" ", notes.Tags.Select(t => t.Title).ToList());
            }
        
            ViewData["Surah"] = await _context.Surah.ToListAsync();
            ViewData["Topics"] = await _context.Topics.ToListAsync();
            ViewData["ViewMode"] = "Edit";
            ViewData["Tags"] = tagsList;
            return View("AddEdit", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,
                                             string submitType, 
                                             ICollection<int> selectedTopics, 
                                             IFormFile notesFile,
                                             string tags,
                                             [Bind("ID,Title,Description,Surah,Ruku,URL,DateAdded,LastUpdated,Topics,Tags")] Notes notes, 
                                             [Bind("AddTopicText, DeleteTopicId, EditTopicId, EditTopicText")] TopicModification topicMod)
        {
            if (id != notes.ID)
                return NotFound();
            
            // Verify action type
            if(submitType != "Add Notes" && submitType != "Edit Notes")
            {
                var newId = await ModifyTopics(submitType, topicMod);
                if (submitType == "Add Topic" && newId != null)
                    selectedTopics.Add((int)newId);
            }

            var surah = await _context.Surah.ToListAsync();
            var topics = await _context.Topics.ToListAsync();


            // if save, save
            if (submitType == "Edit Notes")
            {
                // Update times
                notes.LastUpdated = DateTime.Now;

                // Must update model BEFORE making modifying topics and tags
                _context.Update(notes);
                await _context.SaveChangesAsync();

                // Update notes model
                notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).FirstOrDefaultAsync(n => n.ID == id);

                // Update notes topics
                notes.Topics.RemoveAll(nt => !selectedTopics.Contains(nt.ID));
                var newTopicsIds = selectedTopics.Where(st => !notes.Topics.Any(nt => nt.ID == st));
                notes.Topics.AddRange(topics.Where(t => newTopicsIds.Contains(t.ID)));
                
                // Update Tags
                notes = await UpdateTags(notes, tags);

                // Update URL if new file
                if (notesFile != null)
                    notes.URL = await AzureBlobService.UpdateExistingBlob(notesFile, notes.URL);

                // Update DB
                _context.Update(notes);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            } 
            
            // Update topics
            notes.Topics = topics.Where(t => selectedTopics.Contains(t.ID)).ToList();
            ViewData["Surah"] = surah;
            ViewData["Topics"] = topics.OrderBy(t => t.Title).ToList();
            ViewData["Tags"] = tags;
            ViewData["ViewMode"] = "Edit";
            return View("AddEdit", notes);
        }
        #endregion

        #region Details
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
        #endregion

        #region Delete
        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notes = await _context.Notes.Include(n => n.Topics).Include(n => n.Tags).FirstOrDefaultAsync(m => m.ID == id);
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
            var url = notes.URL;
            _context.Notes.Remove(notes);
            await _context.SaveChangesAsync();
            await AzureBlobService.DeleteBlob(url);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Private Methods
        private async Task<int?> ModifyTopics(string submitType, TopicModification topicMod)
        {
            switch(submitType)
            {
                case "Add Topic":
                    var newTopicText = topicMod.AddTopicText;
                    // If no text, ignore
                    if (String.IsNullOrEmpty(newTopicText))
                    {
                        ViewData["TopicModError"] = $"No name was given for the new topic!";
                        break;
                    }
                    // If topic already exists, ignore
                    if (await _context.Topics.AnyAsync(t => t.Title.ToLower() == newTopicText.ToLower()))
                    {
                        ViewData["TopicModError"] = $"The topic {newTopicText} already exists!";
                        break;
                    }


                    // Add topics to DB
                    var newTopic = new Topics() {Title = topicMod.AddTopicText};
                    await _context.Topics.AddAsync(newTopic);
                    await _context.SaveChangesAsync();

                    // Add new topic ID to selected topics list
                    var newId = (await _context.Topics.FirstAsync(t => t.Title == topicMod.AddTopicText)).ID;
                    return newId;

                case "Delete Topic":
                    // Find Topic
                    var deleteTopicId = topicMod.DeleteTopicId;
                    var topic = await _context.Topics.FirstOrDefaultAsync(t => t.ID == deleteTopicId);

                    // If not found, error
                    if (topic == null)
                    {
                        ViewData["TopicModError"] = $"The topic could not be found!";
                        break;
                    }

                    // Remove it
                    _context.Topics.Remove(topic);
                    await _context.SaveChangesAsync();
                    break;

                case "Edit Topic":
                    var editTopicId = topicMod.EditTopicId;
                    var editTopicText = topicMod.EditTopicText;

                    // If no text, ignore
                    if (String.IsNullOrEmpty(editTopicText))
                    {
                        ViewData["TopicModError"] = $"No name was given for the new topic!";
                        break;
                    }
                    // Make sure text isn't duplicate
                    if (await _context.Topics.AnyAsync(t => t.Title.ToLower() == editTopicText.ToLower()))
                    {
                        ViewData["TopicModError"] = $"The topic {editTopicText} already exists!";
                        break;
                    }

                    // Get topic
                    var targetTopic = await _context.Topics.FirstOrDefaultAsync(t => t.ID == editTopicId);

                    // Modify topic and save
                    targetTopic.Title = editTopicText;
                    await _context.SaveChangesAsync();
                    break;
                default:
                    break;
            }

            return null;
        }
        
        private async Task<Notes> UpdateTags(Notes notes, string tagString)
        {
            if (String.IsNullOrEmpty(tagString))
                return notes;

            // Get current 
            var tags = await _context.Tags.ToListAsync();

            // Get selected tags
            var selectedTagsText = tagString.ToLower().Split(" ").Where(t => !String.IsNullOrEmpty(t)).Distinct();
            var selectedTags = new List<Tags>();
            var newTags = new List<Tags>();
            foreach (var tag in selectedTagsText)
            {
                var existingTag = tags.FirstOrDefault(t => t.Title == tag);
                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
                else
                {
                    // Create new 
                    var newTag = new Tags() {Title = tag};
                    newTags.Add(newTag);
                }
            }
            // Add all new tags
            _context.Tags.AddRange(newTags);
            await _context.SaveChangesAsync();

            // Update tags list
            var updatedTags = await _context.Tags.ToListAsync();

            // Update selected tags list
            selectedTags.AddRange(updatedTags.Where(ut => newTags.Select(nt => nt.Title).Contains(ut.Title)));
            
            // If notes tags is empty, create a new tag list
            if (notes.Tags == null) notes.Tags = new List<Tags>();

            // If notes tags currently has something which selected tags does not, remove it
            notes.Tags.RemoveAll(nt => !selectedTags.Contains(nt));
            notes.Tags.AddRange(selectedTags.Where(st => !notes.Tags.Contains(st)));

            return notes;
        }
        
        #endregion
    }

    public class TopicModification
    {
        public string AddTopicText {get; set;}
        public int DeleteTopicId {get; set;}
        public int EditTopicId {get; set;}
        public string EditTopicText {get; set;}
    }
}
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Models;
using Microsoft.Extensions.Configuration;
using AlMualim.Services;
using Microsoft.AspNetCore.Authorization;
using FuzzySharp;

namespace AlMualim.Controllers
{
    public class AdminController : Controller
    {
        private readonly AlMualimDbContext _context;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionService _subscriptionService;

        private const int WEIGHT_LIMIT = 75;

        public AdminController(AlMualimDbContext context, IAzureBlobService azureBlobService, IConfiguration configuration, ISubscriptionService subscription)
        {
            _context = context;
            _azureBlobService = azureBlobService;
            _configuration = configuration;
            _subscriptionService = subscription;
        }

        #region Index
        [Authorize]
        public async Task<IActionResult> Index(int? surah, int? ruku, int? topic, string searchString)
        {
            // Get Surah and Topics list
            var surahs = Surah.List(); 
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

            // Add Prophets list
            ViewData["Stories"] = await _context.Stories.Include(s => s.Notes).Where(s => s.Notes.Count() > 0).OrderByDescending(s => s.Order).ToListAsync();
            return View(notes);
        }
        #endregion

        #region Add Notes
        [Authorize]
        public async Task<IActionResult> Add()
        {
            ViewData["Surah"] = Surah.List();
            ViewData["Topics"] = await _context.Topics.OrderByDescending(t => t.Order).ToListAsync();
            ViewData["Stories"] = await _context.Stories.OrderByDescending(s => s.Order).ToListAsync();
            ViewData["ViewMode"] = "Add";
            return View("AddEdit");
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(string submitType, 
                                             ICollection<int> selectedTopics, 
                                             int selectedStory,
                                             IFormFile notesFile,
                                             string tags,
                                             [Bind("ID,Title,Description,Surah,Ruku,URL,DateAdded,LastUpdated,IsHistory")] Notes notes, 
                                             [Bind("AddTopicText, DeleteTopicId, EditTopicId, EditTopicText, OrderTopicList, AddStoryText, DeleteStoryId, EditStoryId, EditStoryText, OrderProphetsList")] TopicStoryModification mod)
        {
            // Verify action type
            if(submitType != "Add Notes" && submitType != "Edit Notes")
            {
                var newId = await ModifyStoriesAndTopics(submitType, mod);
                if (newId != null && submitType == "Add Topic")
                    selectedTopics.Add((int)newId);
            }

            var surah = Surah.List();
            var topics = await _context.Topics.ToListAsync();
            var stories = await _context.Stories.ToListAsync();

            // Update topics
            notes.Topics = topics.Where(t => selectedTopics.Contains(t.ID)).ToList();

            // Update story
            notes.Story = stories.FirstOrDefault(s => s.ID == selectedStory);

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
                    notes.URL = await _azureBlobService.UploadNotesAndGetURL(notesFile);

                // Submit if model state is valid
                await _context.Notes.AddAsync(notes);
                await _context.SaveChangesAsync();

                // Send subscription mail
                _subscriptionService.Broadcast(notes);

                return RedirectToAction(nameof(Index));
            } 

            ViewData["Surah"] = surah;
            ViewData["Topics"] = topics.OrderByDescending(t => t.Order).ToList();
            ViewData["Stories"] = stories.OrderByDescending(s => s.Order).ToList();
            ViewData["Tags"] = tags;
            ViewData["ViewMode"] = "Add";
            return View("AddEdit", notes);
        }
        #endregion

        #region Edit
        [Authorize]
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
        
            ViewData["Surah"] = Surah.List();
            ViewData["Topics"] = await _context.Topics.OrderByDescending(t => t.Order).ToListAsync();
            ViewData["Stories"] = await _context.Stories.OrderByDescending(s => s.Order).ToListAsync();
            ViewData["ViewMode"] = "Edit";
            ViewData["Tags"] = tagsList;
            return View("AddEdit", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int? id,
                                             string submitType, 
                                             ICollection<int> selectedTopics, 
                                             int selectedStory,
                                             IFormFile notesFile,
                                             string tags,
                                             [Bind("ID,Title,Description,Surah,Ruku,URL,DateAdded,LastUpdated,Topics,Tags,IsHistory")] Notes notes, 
                                             [Bind("AddTopicText, DeleteTopicId, EditTopicId, EditTopicText, OrderTopicList, AddStoryText, DeleteStoryId, EditStoryId, EditStoryText, OrderProphetsList")] TopicStoryModification mod)
        {
            if (id != notes.ID)
                return NotFound();
            
            // Verify action type
            if(submitType != "Add Notes" && submitType != "Edit Notes")
            {
                var newId = await ModifyStoriesAndTopics(submitType, mod);
                if (submitType == "Add Topic" && newId != null)
                    selectedTopics.Add((int)newId);
            }

            var surah = Surah.List();
            var topics = await _context.Topics.ToListAsync();
            var stories = await _context.Stories.ToListAsync();


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
                    notes.URL = await _azureBlobService.UpdateExistingBlob(notesFile, notes.URL);

                // Update story
                notes.Story = stories.FirstOrDefault(s => s.ID == selectedStory);

                // Update DB
                _context.Update(notes);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            } 
            
            // Update topics
            notes.Topics = topics.Where(t => selectedTopics.Contains(t.ID)).ToList();
            ViewData["Surah"] = surah;
            ViewData["Topics"] = topics.OrderByDescending(t => t.Order).ToList();
            ViewData["Stories"] = stories.OrderByDescending(s => s.Order).ToList();
            ViewData["Tags"] = tags;
            ViewData["ViewMode"] = "Edit";
            return View("AddEdit", notes);
        }
        #endregion

        #region Details
        // GET: Notes/Details/5
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notes = await _context.Notes.FindAsync(id);
            var url = notes.URL;
            _context.Notes.Remove(notes);
            await _context.SaveChangesAsync();
            await _azureBlobService.DeleteBlob(url);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Cleanup
        [Authorize]
        public async Task<IActionResult> Cleanup()
        {
            // Delete all topics not associated with a note
            var topicsWithoutNotes = await _context.Topics.Include(t => t.Notes).Where(t => t.Notes.Count == 0).ToListAsync();
            _context.Topics.RemoveRange(topicsWithoutNotes);

            // Delete all tags not associates with a note
            var tagsWithoutNotes = await _context.Tags.Include(t => t.Notes).Where(t => t.Notes.Count == 0).ToListAsync();
            _context.Tags.RemoveRange(tagsWithoutNotes);

            // Save database
            await _context.SaveChangesAsync();

            // Delete all blobs not associated with a note
            var allUrls = await _context.Notes.Select(n => n.URL).ToListAsync();
            await _azureBlobService.DeleteAllExtraData(allUrls);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Reorder
        [Authorize]
        public async Task<IActionResult> Reorder(string mode, int? storyid)
        {
            ViewData["Surah"] = Surah.List();
            ViewData["Mode"] = mode;

            if (mode == "History")
            {
                var historyNotes = await _context.Notes.Where(n => n.IsHistory == true).OrderByDescending(n => n.HistoryOrder).ToListAsync();
                ViewData["Header"] = "Reorder History Notes";
                return View(historyNotes);
            }
            else if (mode == "Prophet")
            {
                if(storyid == null)
                    return RedirectToAction("Index");
                
                var prophet = await _context.Stories.Include(s => s.Notes).FirstOrDefaultAsync(p => p.ID == storyid);
                if(prophet == null)
                    return RedirectToAction("Index");

                ViewData["Header"] = "Reorder Notes for " + prophet.Prophet;
                return View(prophet.Notes.OrderByDescending(n => n.StoryOrder));
            }

            return RedirectToAction("Index");
        }



        [Authorize]
        [HttpPost, ActionName("Reorder")]
        public async Task<IActionResult> Reorder(string mode, int? storyid, string OrderText)
        {
            if (String.IsNullOrEmpty(mode) || String.IsNullOrEmpty(OrderText))
                return RedirectToAction("Index");

            var notes = await _context.Notes.ToListAsync();
            
            var idList = OrderText.Split(",").Select(c => Convert.ToInt32(c)).ToList();
            var index = idList.Count();
            foreach(var id in idList)
            {
                index--;
                var targetNote = notes.FirstOrDefault(n => n.ID == id);
                if (targetNote != null)
                {
                    if(mode == "History")
                    {
                        targetNote.HistoryOrder = index;
                    }
                    else if(mode == "Prophet")
                    {
                        targetNote.StoryOrder = index;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Password Reset
        [Authorize]
        public async Task<IActionResult> ResetPassword(string password)
        {
            var username = _configuration.GetValue<string>("BaseUserName");
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == username);
            var hasher = new PasswordHasher<IdentityUser>();
            var hashed = hasher.HashPassword(user, password);
            user.PasswordHash = hashed;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion

        #region Private Methods
        private async Task<int?> ModifyStoriesAndTopics(string submitType, TopicStoryModification topicMod)
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

                case "Reorder Topics":
                    var reorderText = topicMod.OrderTopicList;
                    if (String.IsNullOrEmpty(reorderText))
                        break;

                    var orderList = reorderText.Split(",").Select(t => Convert.ToInt32(t)).ToList();
                    var allTopics = await _context.Topics.ToListAsync();

                    var orderIndex = orderList.Count();
                    for(int i = 0; i < orderList.Count(); i++)
                    {
                        orderIndex--;
                        var topicId = orderList[i];
                        var t = allTopics.FirstOrDefault(t => t.ID == topicId);
                        if (t != null)
                            t.Order = orderIndex;
                    }
                    
                    await _context.SaveChangesAsync();
                    break;

                case "Add Prophet":
                    var newStoryText = topicMod.AddStoryText;
                    // If no text, ignore
                    if (String.IsNullOrEmpty(newStoryText))
                    {
                        ViewData["TopicModError"] = $"No name was given for the new Story!";
                        break;
                    }
                    // If topic already exists, ignore
                    if (await _context.Stories.AnyAsync(s => s.Prophet.ToLower() == newStoryText.ToLower()))
                    {
                        ViewData["TopicModError"] = $"{newStoryText} already exists!";
                        break;
                    }

                    // Add topics to DB
                    var newStory = new Stories() {Prophet = newStoryText};
                    await _context.Stories.AddAsync(newStory);
                    await _context.SaveChangesAsync();
                    break;

                case "Delete Prophet":
                    // Find Topic
                    var deleteStoryId = topicMod.DeleteStoryId;
                    var story = await _context.Stories.Include(s => s.Notes).FirstOrDefaultAsync(t => t.ID == deleteStoryId);

                    // If not found, error
                    if (story == null)
                    {
                        ViewData["TopicModError"] = $"The story could not be found!";
                        break;
                    }

                    // Remove it
                    _context.Stories.Remove(story);
                    await _context.SaveChangesAsync();
                    break;

                case "Edit Prophet":
                    var editStoryId = topicMod.EditStoryId;
                    var editStoryText = topicMod.EditStoryText;

                    // If no text, ignore
                    if (String.IsNullOrEmpty(editStoryText))
                    {
                        ViewData["TopicModError"] = $"No name was given for the new story!";
                        break;
                    }
                    // Make sure text isn't duplicate
                    if (await _context.Stories.AnyAsync(t => t.Prophet.ToLower() == editStoryText.ToLower()))
                    {
                        ViewData["TopicModError"] = $"{editStoryText} already exists!";
                        break;
                    }

                    // Get topic
                    var targetStory = await _context.Stories.FirstOrDefaultAsync(t => t.ID == editStoryId);

                    // Modify topic and save
                    targetStory.Prophet = editStoryText;
                    await _context.SaveChangesAsync();
                    break;

                case "Reorder Prophets":
                    var reorderProphetText = topicMod.OrderProphetsList;
                    if (String.IsNullOrEmpty(reorderProphetText))
                        break;

                    var orderStoryList = reorderProphetText.Split(",").Select(t => Convert.ToInt32(t)).ToList();
                    var allStories = await _context.Stories.ToListAsync();

                    var orderStoryIndex = orderStoryList.Count();
                    for(int i = 0; i < orderStoryList.Count(); i++)
                    {
                        orderStoryIndex--;
                        var storyId = orderStoryList[i];
                        var s = allStories.FirstOrDefault(t => t.ID == storyId);
                        if (s != null)
                            s.Order = orderStoryIndex;
                    }
                    
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

    public class TopicStoryModification
    {
        public string AddTopicText {get; set;}
        public int DeleteTopicId {get; set;}
        public int EditTopicId {get; set;}
        public string EditTopicText {get; set;}
        public string OrderTopicList {get; set;}

        public string AddStoryText {get; set;}
        public int DeleteStoryId {get; set;}
        public int EditStoryId {get; set;}
        public string EditStoryText {get; set;}
        public string OrderProphetsList {get; set;}
    }
}
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

        // GET: Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topics = await _context.Topics
                .FirstOrDefaultAsync(m => m.ID == id);
            if (topics == null)
            {
                return NotFound();
            }

            return View(topics);
        }

        // GET: Topics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Topics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title")] Topics topics)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topics);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topics);
        }

        // GET: Topics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topics = await _context.Topics.FindAsync(id);
            if (topics == null)
            {
                return NotFound();
            }
            return View(topics);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title")] Topics topics)
        {
            if (id != topics.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topics);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicsExists(topics.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(topics);
        }

        // GET: Topics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topics = await _context.Topics
                .FirstOrDefaultAsync(m => m.ID == id);
            if (topics == null)
            {
                return NotFound();
            }

            return View(topics);
        }

        // POST: Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topics = await _context.Topics.FindAsync(id);
            _context.Topics.Remove(topics);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopicsExists(int id)
        {
            return _context.Topics.Any(e => e.ID == id);
        }
    }
}

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
    public class HistoryController : Controller
    {

        private readonly AlMualimDbContext _context;

        public HistoryController(AlMualimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var notes = await _context.Notes.Where(n => n.IsHistory == true).OrderByDescending(n => n.HistoryOrder).ToListAsync();
            ViewData["Surah"] = Surah.List();
            return View(notes);
        }
    }
}
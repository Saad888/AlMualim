using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlMualim.Data;
using AlMualim.Models;
using AlMualim.Services;

namespace AlMualim.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly AlMualimDbContext _context;
        private readonly ISubscriptionService _service;

        public SubscriptionController(AlMualimDbContext context, ISubscriptionService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Topics
        public IActionResult Index()
        {
            return View();
        }

        // GET: Unsubscribe
        public async Task<IActionResult> Unsubscribe(string id)
        {
            Guid guid;
            string email = String.Empty;
            var validId = Guid.TryParse(id, out guid);
            if (validId)
            {
                email = await _service.UnsubscribeEmail(Guid.Parse(id));
            }
            ViewData["Email"] = email;
            return View();
        }

        // POST: Subscribe
        [HttpPost]
        public async Task Subscribe([FromBody]SubscribeInput input)
        {
            await _service.SubscribeEmail(input.Name, input.Email);
        }
    }

    public class SubscribeInput{
        public string Name {get; set;}
        public string Email {get; set;}
    }
}

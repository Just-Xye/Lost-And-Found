using LostAndFound.Data;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Controllers
{
    public class LostAndFoundController : Controller
    {
        private readonly LostAndFoundContext _context;
        public LostAndFoundController(LostAndFoundContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

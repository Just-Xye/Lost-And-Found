using LostAndFound.Data;
using LostAndFound.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LostAndFound.Controllers
{
    public class HomeController : Controller
    {
        private readonly LostAndFoundContext _context;
        private readonly string _imageFolder;

        public HomeController(LostAndFoundContext context)
        {
            _context = context;
            _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "lost-images");

            if (!Directory.Exists(_imageFolder))
                Directory.CreateDirectory(_imageFolder);
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult AboutUs() => View();

        // ======================
        // FOUND ITEMS
        // ======================

        public IActionResult ReportFound() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportFound(FoundItemList model, IFormFile? imageFile)
        {
            model.DateFound = DateTime.Now;
            model.IsApproved = false;
            model.ImageName = "default.webp";

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(extension))
                    return BadRequest("Invalid file type");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(_imageFolder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                model.ImageName = fileName;
            }

            _context.Found_Item_Lists.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ReportFound));
        }

        public IActionResult FoundItems()
        {
            var items = _context.Found_Item_Lists
                .Where(i => i.IsApproved)
                .ToList();

            return View(items);
        }

        // ======================
        // LOST ITEMS
        // ======================

        public IActionResult ReportLost() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportLost(LostItemList model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.ImageName = "default.webp";

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(extension))
                    return BadRequest("Invalid file type");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(_imageFolder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                model.ImageName = fileName;
            }

            _context.Lost_Item_Lists.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(FoundItems));
        }

        // ======================
        // ADMIN
        // ======================

        [Authorize]
        public IActionResult AdminDashboard()
        {
            var vm = new AdminDashboardViewModel
            {
                PendingFoundItems = _context.Found_Item_Lists.Where(i => !i.IsApproved).ToList(),
                ApprovedFoundItems = _context.Found_Item_Lists.Where(i => i.IsApproved).ToList(),
                StudentLostReports = _context.Lost_Item_Lists.ToList(),
                ClaimRequests = _context.ClaimRequests.ToList()
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveItem(int id)
        {
            var item = _context.Found_Item_Lists.Find(id);
            if (item != null)
            {
                item.IsApproved = true;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(AdminDashboard));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectItem(int id)
        {
            var item = _context.Found_Item_Lists.Find(id);
            if (item != null)
            {
                _context.Found_Item_Lists.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(AdminDashboard));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveItem(int id)
        {
            var item = await _context.Found_Item_Lists.FindAsync(id);
            if (item != null)
            {
                _context.Found_Item_Lists.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AdminDashboard));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToFound(int id)
        {
            var lost = await _context.Lost_Item_Lists.FindAsync(id);
            if (lost == null) return NotFound();

            var found = new FoundItemList
            {
                ItemName = lost.ItemName,
                Description = lost.Description,
                FoundAt = lost.LocationLost,
                PickupLocation = "Faculty",
                DateFound = DateTime.Now,
                Name = lost.OwnerName,
                Email = lost.Email,
                PhoneNumber = lost.PhoneNumber,
                ImageName = lost.ImageName, // important
                IsApproved = true
            };

            _context.Found_Item_Lists.Add(found);
            _context.Lost_Item_Lists.Remove(lost);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminDashboard));
        }

        // ======================
        // CLAIMS
        // ======================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim([FromBody] ClaimRequests claim)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var item = await _context.Found_Item_Lists.FindAsync(claim.ItemId);
            if (item == null)
                return BadRequest("Item not found");

            if (_context.ClaimRequests.Any(c =>
                c.ItemId == claim.ItemId &&
                c.RequesterName == claim.RequesterName))
            {
                return BadRequest("Duplicate claim");
            }

            claim.ItemName = item.ItemName;
            claim.ImageName = item.ImageName;
            claim.RequestDate = DateTime.Now;

            _context.ClaimRequests.Add(claim);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

	[Authorize]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ApproveClaim(int id)
	{
    	var claim = await _context.ClaimRequests.FindAsync(id);
    	if (claim == null) return NotFound();

    	_context.ClaimRequests.Remove(claim);
    	await _context.SaveChangesAsync();

    	return RedirectToAction(nameof(AdminDashboard));
	}

	[Authorize]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RejectClaim(int id)
	{
    	var claim = await _context.ClaimRequests.FindAsync(id);
    	if (claim != null)
    	{
        	_context.ClaimRequests.Remove(claim);
        	await _context.SaveChangesAsync();
    	}

    return RedirectToAction(nameof(AdminDashboard));
}

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
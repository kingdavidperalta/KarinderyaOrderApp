using KarinderyaOrderApp.Data;
using KarinderyaOrderApp.Models;
using KarinderyaOrderApp.ViewModels.Food;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarinderyaOrderApp.Controllers
{
    public class FoodController : Controller
    {
        private readonly AppDbContext _db;

        public FoodController(AppDbContext db) => this._db = db;

        public async Task<IActionResult> Index(string? searchTerm, string tab = "active", int page = 1)
        {
            var isArchived = string.Equals(tab, "archived", StringComparison.OrdinalIgnoreCase);
            var term = searchTerm?.Trim();

            var query = _db.Foods.AsNoTracking();

            if (!string.IsNullOrEmpty(term))
                query = query.Where(f => f.Name.Contains(term) || f.Description.Contains(term));

            var activeCount = await query.CountAsync(f => !f.IsArchived);
            var archivedCount = await query.CountAsync(f => f.IsArchived);

            var vm = new IndexFoodViewModel
            {
                Tab = isArchived ? "archived" : "active",
                SearchTerm = term,
                ActiveCount = activeCount,
                ArchivedCount = archivedCount,
                TotalCount = isArchived ? archivedCount : activeCount
            };

            vm.Page = Math.Clamp(page, 1, vm.TotalPages);

            vm.Items = await query
                .Where(f => f.IsArchived == isArchived)
                .OrderBy(f => f.Name)
                .Skip((vm.Page - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .Select(f => new ListFoodItemViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    QuantityInStock = f.QuantityInStock,
                    IsArchived = f.IsArchived
                })
                .ToListAsync();

            return View(vm);
        }

        public IActionResult Create() => View(new CreateFoodViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFoodViewModel vm)
        {
            vm.Name = vm.Name?.Trim() ?? string.Empty;

            if (await _db.Foods.AnyAsync(f => f.Name.ToLower() == vm.Name.ToLower()))
                ModelState.AddModelError(nameof(vm.Name), "A food with this name already exists.");

            if (!ModelState.IsValid) return View(vm);

            var food = new Food
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                QuantityInStock = vm.QuantityInStock
            };

            try
            {
                _db.Foods.Add(food);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Could not save. The name may already exist, please try again.");
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _db.Foods.AsNoTracking()
                .Where(f => f.Id == id)
                .Select(f => new EditFoodViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    QuantityInStock = f.QuantityInStock
                })
                .FirstOrDefaultAsync();

            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditFoodViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            vm.Name = vm.Name?.Trim() ?? string.Empty;

            if (await _db.Foods.AnyAsync(f => f.Name.ToLower() == vm.Name.ToLower() && f.Id != vm.Id))
                ModelState.AddModelError(nameof(vm.Name), "A food with this name already exists.");

            if (!ModelState.IsValid) return View(vm);

            var food = await _db.Foods.FindAsync(id);
            if (food == null) return NotFound();

            food.Name = vm.Name;
            food.Description = vm.Description;
            food.Price = vm.Price;
            food.QuantityInStock = vm.QuantityInStock;

            try
            {
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Could not save. The name may already exist, please try again.");
                return View(vm);
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            var food = await _db.Foods.FindAsync(id);
            if (food != null)
            {
                food.IsArchived = true;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var food = await _db.Foods.FindAsync(id);
            if (food != null)
            {
                food.IsArchived = false;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { tab = "archived" });
        }
    }
}
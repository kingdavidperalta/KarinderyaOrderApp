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

    }
}
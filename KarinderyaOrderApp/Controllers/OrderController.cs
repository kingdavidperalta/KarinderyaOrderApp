using KarinderyaOrderApp.Data;
using KarinderyaOrderApp.Models;
using KarinderyaOrderApp.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarinderyaOrderApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;

        public OrderController(AppDbContext db) => this._db = db;

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            var term = searchTerm?.Trim();
            var query = _db.Orders.AsNoTracking();

            if (!string.IsNullOrEmpty(term))
            {
                var hasId = int.TryParse(term.TrimStart('#'), out var orderId);
                query = query.Where(o =>
                    (hasId && o.Id == orderId) ||
                    o.Status.Contains(term) ||
                    o.OrderItems.Any(i => i.Food.Name.Contains(term)));
            }

            var vm = new IndexOrderViewModel
            {
                SearchTerm = term,
                TotalCount = await query.CountAsync()
            };

            vm.Page = Math.Clamp(page, 1, vm.TotalPages);

            vm.Items = await query
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Skip((vm.Page - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .Select(o => new ListOrderItemViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalQuantity = o.OrderItems.Sum(i => i.Quantity),
                    TotalPrice = o.OrderItems.Sum(i => i.Quantity * i.UnitPrice)
                })
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vm = await _db.Orders.AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new DetailsOrderViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Lines = o.OrderItems.Select(i => new DetailsOrderLineViewModel
                    {
                        FoodName = i.Food.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (vm == null) return NotFound();
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateOrderViewModel();
            await LoadLinesAsync(vm);
            return View(vm);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel vm)
        {
            var errors = new List<string>();

            if (!ModelState.IsValid)
                errors.Add("Please check the quantities and try again.");

            // Keep only the foods the customer actually ordered (quantity above 0).
            // If the same food appears more than once, add the quantities together.
            var wanted = vm.Lines
                .Where(l => l.Quantity > 0)
                .GroupBy(l => l.FoodId)
                .Select(g => new { 
                    FoodId = g.Key, 
                    Quantity = g.Sum(l => l.Quantity) 
                })
                .ToList();

            if (!wanted.Any() && errors.Count == 0)
                errors.Add("Add at least one item to the order.");

            if (errors.Count == 0)
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    var ids = wanted.Select(w => w.FoodId).ToList();

                    var foods = await _db.Foods.AsNoTracking()
                        .Where(f => ids.Contains(f.Id) && !f.IsArchived)
                        .ToListAsync();

                    var order = new Order
                    {
                        OrderDate = DateTime.UtcNow,
                        Status = "Pending"
                    };

                    foreach (var line in wanted)
                    {
                        var food = foods.FirstOrDefault(f => f.Id == line.FoodId);
                        if (food == null)
                        {
                            errors.Add("One of the selected foods is no longer available.");
                            break;
                        }

                        // Check and reduce stock in one statement (safe if two people order at once)
                        var updated = await _db.Foods
                            .Where(f => f.Id == line.FoodId
                                     && !f.IsArchived
                                     && f.QuantityInStock >= line.Quantity)
                            .ExecuteUpdateAsync(s => s.SetProperty(
                                f => f.QuantityInStock,
                                f => f.QuantityInStock - line.Quantity));

                        if (updated == 0)
                        {
                            errors.Add($"Not enough stock for {food.Name}.");
                            break;
                        }

                        order.OrderItems.Add(new OrderItem
                        {
                            FoodId = line.FoodId,
                            Quantity = line.Quantity,
                            UnitPrice = food.Price   
                        });
                    }

                    if (errors.Count > 0)
                    {
                        await transaction.RollbackAsync();
                    }
                    else
                    {
                        _db.Orders.Add(order);
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return RedirectToAction(nameof(Details), new { id = order.Id });
                    }
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            ModelState.Clear();
            foreach (var e in errors) ModelState.AddModelError(string.Empty, e);
            await LoadLinesAsync(vm);
            return View(vm);
        }

       
        private async Task LoadLinesAsync(CreateOrderViewModel vm)
        {
            var typed = vm.Lines
                .GroupBy(l => l.FoodId)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Quantity));

            vm.Lines = await _db.Foods.AsNoTracking()
                .Where(f => !f.IsArchived && f.QuantityInStock > 0)
                .OrderBy(f => f.Name)
                .Select(f => new CreateOrderLineViewModel
                {
                    FoodId = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    QuantityInStock = f.QuantityInStock
                })
                .ToListAsync();

            foreach (var line in vm.Lines)
                if (typed.TryGetValue(line.FoodId, out var qty))
                    line.Quantity = qty;
        }
    }
}
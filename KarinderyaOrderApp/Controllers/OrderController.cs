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

    
    }
}
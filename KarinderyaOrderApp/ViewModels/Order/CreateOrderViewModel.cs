using System.ComponentModel.DataAnnotations;

namespace KarinderyaOrderApp.ViewModels.Order
{
    public class CreateOrderViewModel
    {
        public List<CreateOrderLineViewModel> Lines { get; set; } = new();
    }

    public class CreateOrderLineViewModel
    {
       
        public int FoodId { get; set; }

        [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000.")]
        public int Quantity { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}

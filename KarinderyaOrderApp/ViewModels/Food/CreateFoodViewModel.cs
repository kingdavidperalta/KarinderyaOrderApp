using System.ComponentModel.DataAnnotations;

namespace KarinderyaOrderApp.ViewModels.Food
{
    public class CreateFoodViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock can't be negative.")]
        public int QuantityInStock { get; set; }
    }
}

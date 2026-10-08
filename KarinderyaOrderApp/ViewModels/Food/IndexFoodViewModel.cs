namespace KarinderyaOrderApp.ViewModels.Food
{
    public class IndexFoodViewModel
    {
        public List<ListFoodItemViewModel> Active { get; set; } = new();
        public List<ListFoodItemViewModel> Archived { get; set; } = new();
    }

    public class ListFoodItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsInStock => QuantityInStock > 0;
        public bool IsArchived { get; set; }
    }
}

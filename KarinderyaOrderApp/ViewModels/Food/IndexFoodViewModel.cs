using KarinderyaOrderApp.ViewModels.Page;

namespace KarinderyaOrderApp.ViewModels.Food
{
    public class IndexFoodViewModel: PagedViewModel
    {
        public string Tab { get; set; } = "active";
        public bool IsArchivedTab => Tab == "archived";
        public int ActiveCount { get; set; }
        public int ArchivedCount { get; set; }

        public List<ListFoodItemViewModel> Items { get; set; } = new();
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

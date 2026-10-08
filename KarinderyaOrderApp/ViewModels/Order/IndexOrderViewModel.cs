using KarinderyaOrderApp.ViewModels.Page;

namespace KarinderyaOrderApp.ViewModels.Order
{
    public class IndexOrderViewModel : PagedViewModel
    {
        public List<ListOrderItemViewModel> Items { get; set; } = new();
    }
    public class ListOrderItemViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
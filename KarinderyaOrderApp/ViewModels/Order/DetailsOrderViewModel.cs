namespace KarinderyaOrderApp.ViewModels.Order
{
    public class DetailsOrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<DetailsOrderLineViewModel> Lines { get; set; } = new();

        public int TotalQuantity => Lines.Sum(l => l.Quantity);
        public decimal TotalPrice => Lines.Sum(l => l.LineTotal);
    }
    public class DetailsOrderLineViewModel
    {
        public string FoodName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}

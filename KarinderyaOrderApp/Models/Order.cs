using System.ComponentModel.DataAnnotations.Schema;

namespace KarinderyaOrderApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [NotMapped]
        public int TotalQuantity => OrderItems.Sum(i => i.Quantity);

        [NotMapped]
        public decimal TotalPrice => OrderItems.Sum(i => i.LineTotal);
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string ProductCode { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice => Quantity * UnitPrice;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

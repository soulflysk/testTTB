using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models
{
    public class Product
    {
        [Key]
        [Required]
        [MaxLength(20)]
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
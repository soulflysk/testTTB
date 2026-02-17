using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models
{
    public class Stock
    {
        [Key, ForeignKey("Product")]
        [MaxLength(20)]
        public string ProductCode { get; set; }

        public int Quantity { get; set; }

        public Product Product { get; set; }
    }
}
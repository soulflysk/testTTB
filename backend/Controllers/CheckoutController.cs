using DOTNETWEBAPI_DEV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly DbContextClass _context;

        public CheckoutController(DbContextClass context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            // Get all cart items
            var cartItems = await _context.ShoppingCarts.ToListAsync();

            if (!cartItems.Any())
                return BadRequest("ตะกร้าสินค้าว่างเปล่า");

            // Clear cart (สต็อกถูกตัดไปแล้วตอนเพิ่มลงตะกร้า)
            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Calculate total
            var totalAmount = cartItems.Sum(c => c.TotalPrice);

            return Ok(new { 
                message = "ชำระเงินสำเร็จ",
                totalAmount = totalAmount,
                itemsProcessed = cartItems.Count,
                timestamp = DateTime.UtcNow,
                note = "สต็อกถูกตัดไปแล้วเมื่อเพิ่มสินค้าลงตะกร้า"
            });
        }

        [HttpGet("stock-status")]
        public async Task<IActionResult> GetStockStatus()
        {
            var stocks = await _context.Stocks
                .Include(s => s.Product)
                .ToListAsync();

            var stockStatus = stocks.Select(s => new
            {
                productCode = s.ProductCode,
                productName = s.Product?.ProductName ?? "N/A",
                currentStock = s.Quantity,
                lastUpdated = DateTime.UtcNow
            }).ToList();

            return Ok(stockStatus);
        }
    }

    public class CheckoutRequest
    {
        public string PaymentMethod { get; set; } = "cash";
        public string CustomerInfo { get; set; }
    }
}

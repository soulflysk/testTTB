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

            // Process each item - update stock
            var errors = new List<string>();
            
            foreach (var item in cartItems)
            {
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.ProductCode == item.ProductCode);

                if (stock == null)
                {
                    errors.Add($"ไม่พบข้อมูลสต็อกสำหรับสินค้า {item.ProductCode}");
                    continue;
                }

                if (stock.Quantity < item.Quantity)
                {
                    errors.Add($"สินค้า {item.ProductCode} มีสต็อกไม่เพียงพอ (มี {stock.Quantity}, ต้องการ {item.Quantity})");
                    continue;
                }

                // Deduct stock
                stock.Quantity -= item.Quantity;
            }

            if (errors.Any())
                return BadRequest(new { message = "ไม่สามารถดำเนินการชำระเงินได้", errors });

            // Save stock changes
            await _context.SaveChangesAsync();

            // Clear cart
            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Calculate total
            var totalAmount = cartItems.Sum(c => c.TotalPrice);

            return Ok(new { 
                message = "ชำระเงินสำเร็จ",
                totalAmount = totalAmount,
                itemsProcessed = cartItems.Count,
                timestamp = DateTime.UtcNow
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

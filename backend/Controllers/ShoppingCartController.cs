using DOTNETWEBAPI_DEV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly DbContextClass _context;

        public ShoppingCartController(DbContextClass context)
        {
            _context = context;
        }

        // GET: api/shoppingcart
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var cartItems = await _context.ShoppingCarts
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Get product info separately to avoid circular reference
            var cartItemsDto = new List<object>();
            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductCode == item.ProductCode);
                
                cartItemsDto.Add(new
                {
                    id = item.Id,
                    productCode = item.ProductCode,
                    quantity = item.Quantity,
                    unitPrice = item.UnitPrice,
                    totalPrice = item.TotalPrice,
                    createdAt = item.CreatedAt,
                    product = product != null ? new
                    {
                        productCode = product.ProductCode,
                        productName = product.ProductName,
                        unitPrice = product.UnitPrice
                    } : null
                });
            }

            var cartSummary = new
            {
                Items = cartItemsDto,
                TotalItems = cartItems.Sum(c => c.Quantity),
                TotalAmount = cartItems.Sum(c => c.TotalPrice)
            };

            return Ok(cartSummary);
        }

        // POST: api/shoppingcart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            // Get product and stock info separately
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode);

            if (product == null)
                return NotFound("ไม่พบสินค้า");

            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductCode == request.ProductCode);

            if (stock == null || stock.Quantity < request.Quantity)
                return BadRequest("สินค้าในสต็อกไม่เพียงพอ");

            var existingCartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.ProductCode == request.ProductCode);

            if (existingCartItem != null)
            {
                var newQuantity = existingCartItem.Quantity + request.Quantity;
                if (stock.Quantity < newQuantity)
                    return BadRequest("สินค้าในสต็อกไม่เพียงพอสำหรับจำนวนที่รวมกัน");

                existingCartItem.Quantity = newQuantity;
                existingCartItem.UnitPrice = product.UnitPrice;
            }
            else
            {
                var cartItem = new ShoppingCart
                {
                    ProductCode = request.ProductCode,
                    Quantity = request.Quantity,
                    UnitPrice = product.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ShoppingCarts.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Return simple success response
            return Ok(new { message = "เพิ่มสินค้าลงตะกร้าเรียบร้อย", success = true });
        }

        // PUT: api/shoppingcart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartRequest request)
        {
            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cartItem == null)
                return NotFound();

            // Get product and stock info separately
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductCode == cartItem.ProductCode);

            if (stock == null || stock.Quantity < request.Quantity)
                return BadRequest("สินค้าในสต็อกไม่เพียงพอ");

            cartItem.Quantity = request.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "อัปเดตจำนวนสินค้าเรียบร้อย", success = true });
        }

        // DELETE: api/shoppingcart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.ShoppingCarts.FindAsync(id);
            if (cartItem == null)
                return NotFound();

            _context.ShoppingCarts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(await GetCartItems());
        }

        // DELETE: api/shoppingcart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            _context.ShoppingCarts.RemoveRange(_context.ShoppingCarts);
            await _context.SaveChangesAsync();

            return Ok(new { message = "ตะกร้าสินค้าถูกล้างแล้ว" });
        }
    }

    public class AddToCartRequest
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int Quantity { get; set; }
    }
}

using DOTNETWEBAPI_DEV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbContextClass _context;

        public ProductController(DbContextClass context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .ToListAsync();

            // Get stock info separately to avoid circular reference
            var productsDto = new List<object>();
            foreach (var product in products)
            {
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.ProductCode == product.ProductCode);
                
                productsDto.Add(new
                {
                    productCode = product.ProductCode,
                    productName = product.ProductName,
                    unitPrice = product.UnitPrice,
                    stock = stock != null ? new
                    {
                        productCode = stock.ProductCode,
                        quantity = stock.Quantity
                    } : null
                });
            }

            return Ok(productsDto);
        }

        // GET: api/product/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> GetById(string code)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == code);

            if (product == null)
                return NotFound();

            // Get stock info separately
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductCode == code);

            var productDto = new
            {
                productCode = product.ProductCode,
                productName = product.ProductName,
                unitPrice = product.UnitPrice,
                stock = stock != null ? new
                {
                    productCode = stock.ProductCode,
                    quantity = stock.Quantity
                } : null
            };

            return Ok(productDto);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.ProductCode == product.ProductCode))
                return BadRequest("รหัสสินค้าซ้ำ");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { code = product.ProductCode }, product);
        }

        // PUT: api/product/{code}
        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, Product product)
        {
            if (code != product.ProductCode)
                return BadRequest();

            var existing = await _context.Products.FindAsync(code);
            if (existing == null)
                return NotFound();

            existing.ProductName = product.ProductName;
            existing.UnitPrice = product.UnitPrice;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/product/{code}
        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var product = await _context.Products.FindAsync(code);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

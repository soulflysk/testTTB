using System.Collections;
using DOTNETWEBAPI_DEV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly DbContextClass _context;

        public StocksController(DbContextClass context)
        {
            _context = context;
        }

        // GET: api/stocks
        [HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            var stocks = await _context.Stocks
                .Include(s => s.Product)
                .ToListAsync();

            return Ok(stocks);
        }

        // POST: api/stocks
        [HttpPost]
        public async Task<IActionResult> CreateStock(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return Ok(stock);
        }

        // PUT: api/stocks/{code}
        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateStock(string code, int quantity)
        {
            var stock = await _context.Stocks.FindAsync(code);
            if (stock == null)
                return NotFound();

            stock.Quantity = quantity;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

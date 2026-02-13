// Controllers/ClaimsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOTNETWEBAPI_DEV.Data;
namespace DOTNETWEBAPI_DEV.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class ClaimsController : ControllerBase
{
    private readonly DbContextClass _context;

    public ClaimsController(DbContextClass context)
    {
        _context = context;
    }

    [HttpGet]
public async Task<IActionResult> GetClaims(
    string? status,
    string? search,
    int page = 1,
    int pageSize = 10)
{
    var query = _context.Claims.AsQueryable();

    // 🔎 Filter by Status
    if (!string.IsNullOrEmpty(status))
    {
        query = query.Where(c => c.Status == status);
    }

    // 🔎 Search (PolicyNo หรือ CustomerName)
    if (!string.IsNullOrEmpty(search))
    {
        search = search.Trim().ToLower();

        query = query.Where(c =>
            c.PolicyNo.ToLower().Contains(search) ||
            c.CustomerName.ToLower().Contains(search)
        );
    }

    var totalCount = await query.CountAsync();

    var data = await query
        .OrderByDescending(c => c.ClaimId)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync();

    return Ok(new
    {
        totalCount,
        page,
        pageSize,
        data
    });
}

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null) return NotFound();
        return Ok(claim);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Claim claim)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = claim.ClaimId }, claim);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Claim claim)
    {
        if (id != claim.ClaimId)
            return BadRequest();

        _context.Entry(claim).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
}
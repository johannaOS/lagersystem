using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lagersystem.Models;
using Lagersystem.DTOs;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly StorageContext _context;
    public ProductsController(StorageContext context)
    {
        _context = context;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(string? category)
    {
        var query = _context.Products.AsQueryable(); //gör att vi kan lägga till villkor innan frågan körs. Filtret blir en del av SQL-frågan, så databasen filtrerar, inte C#-koden.

        //Where läggs bara till om användaren har skickat en kategori.
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        return await query
            .Select( p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Count = p.Count,
            })
            .ToListAsync();
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProducts(int id)
    {
        var products = await _context.Products.FindAsync(id);

        if (products == null)
        {
            return NotFound();
        }

        var dtoProduct = new ProductDto
        {
            Id = products.Id,
            Name = products.Name,
            Price = products.Price,
            Count = products.Count
        };
        return dtoProduct;
    }

    // GET: api/Products/stats
    [HttpGet("stats")]
    public async Task<ActionResult<ProductStatsDto>> GetStats()
    {
       if (!await _context.Products.AnyAsync()) //_context.Products.AnyAsync() skickar en fråga till databasen som stoppar så snart den hittar en rad. Den returnerar true om det finns minst en produkt, annars false.
        {
            return new ProductStatsDto
            {
                TotalProducts = 0,
                TotalInventoryValue = 0,
                AveragePrice = 0
            };
        }

        return new ProductStatsDto
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalInventoryValue = await _context.Products.SumAsync(p =>  p.Price * p.Count),
            AveragePrice = (decimal) await _context.Products.AverageAsync(p => p.Price)
        };
    }

    // PUT: api/Products/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProducts(int? id, CreateProductsDto productDto)
    {
        var products = await _context.Products.FindAsync(id);
        if (products == null)
        {
            return NotFound();
        }

        products.Name = productDto.Name;
        products.Price = productDto.Price;
        products.Category = productDto.Category;
        products.Shelf = productDto.Shelf;
        products.Count = productDto.Count;
        products.Description = productDto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductsExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Products
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProducts(CreateProductsDto dto) // ÄNDRAT: tar emot DTO
    {
        var products = new Products // ÄNDRAT: bygger modellen från DTO:n
        {
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Category,
            Shelf = dto.Shelf,
            Count = dto.Count,
            Description = dto.Description
        };

        _context.Products.Add(products);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProducts", new { id = products.Id }, ToDto(products)); // ÄNDRAT: returnerar DTO
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducts(int? id)
    {
        var products = await _context.Products.FindAsync(id);
        if (products == null)
        {
            return NotFound();
        }

        _context.Products.Remove(products);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductsExists(int? id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    // ÄNDRAT: ny hjälpmetod som mappar modellen till DTO:n
    private static ProductDto ToDto(Products products) => new ProductDto
    {
        Id = products.Id,
        Name = products.Name,
        Price = products.Price,
        Count = products.Count
    };
}

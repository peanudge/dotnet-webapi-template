using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Packt.Shared;


[Route("api/[controller]")]
[ApiController]
public class NorthwindController : ControllerBase
{
    private readonly NorthwindContext _context;

    public NorthwindController(NorthwindContext context)
    {
        _context = context;
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var stringBuilder = new StringBuilder("Categories and how many products they have:");

        IQueryable<Category> cats = _context.Categories.Include(c => c.Products);
        foreach (var c in cats)
        {
            stringBuilder.AppendLine($"{c.CategoryName} has {c.Products.Count} products.");
        }

        return Ok(stringBuilder.ToString());
    }

    [HttpGet("products")]
    public IActionResult GetProducts(decimal price = 0)
    {
        IOrderedEnumerable<Product> prods = _context.Products
            .TagWith("Products filtered by price and sorted.")
            .AsEnumerable()
            .Where(product => product.Cost > price)
            .OrderByDescending(product => product.Cost);

        var stringBuilder = new StringBuilder();

        foreach (Product item in prods)
        {
            stringBuilder.AppendLine(string.Format("{0}: {1} costs {2:$#,##0.00} and has {3} in stock.",
               item.ProductID, item.ProductName, item.Cost, item.Stock));
        }

        var result = stringBuilder.ToString();

        return Ok(result);
    }


    [HttpGet("product")]
    public IActionResult GetProduct(string keyword)
    {
        var stringBuilder = new StringBuilder();
        IQueryable<Product> prods = _context.Products.Where(p => EF.Functions.Like(p.ProductName!, $"%{keyword}%"));
        foreach (Product item in prods)
        {
            stringBuilder.AppendFormat("{0} has {1} units in stock. Discontinued? {2}\n", item.ProductName, item.Stock, item.Discontinued);
        }
        return Ok(stringBuilder.ToString());
    }
}

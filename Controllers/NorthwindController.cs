using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static System.Console;

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

}

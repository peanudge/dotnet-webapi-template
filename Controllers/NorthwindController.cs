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

        IQueryable<Category> cats = _context.Categories;

        _context.ChangeTracker.LazyLoadingEnabled = false;

        foreach (var c in cats)
        {
            var products = _context.Entry(c).Collection(c2 => c2.Products);
            if (!products.IsLoaded) products.Load();
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
        var prod = _context.Products.First(p => EF.Functions.Like(p.ProductName!, $"%{keyword}%"));
        return prod == null ? NotFound() : Ok(ProductToDTO(prod));
    }


    [HttpPost("product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AddProduct([FromBody] ProductToUpdate productToUpdate)
    {
        // TODO: add body
        var newProduct = new Product
        {
            CategoryId = productToUpdate.CategoryID,
            ProductName = productToUpdate.ProductName,
            Cost = productToUpdate.Cost
        };

        _context.Products.Add(newProduct);

        int affected = _context.SaveChanges();
        if (affected == 1)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }


    [HttpPut("product/cost")]
    public IActionResult IncreaseProductPrice(string name, decimal amount)
    {
        var updateProduct = _context.Products.First(p => p.ProductName!.StartsWith(name));

        if (updateProduct == null)
        {
            return NotFound($"Not found product with name({name})");
        }

        updateProduct.Cost += amount;

        int affected = _context.SaveChanges();

        return affected == 1 ? Ok() : BadRequest("Fail");
    }

    private static ProductDTO ProductToDTO(Product product) =>
                new()
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    CategoryId = product.CategoryId,
                    Cost = product.Cost,
                    Stock = product.Stock,
                    Discontinued = product.Discontinued
                };
}
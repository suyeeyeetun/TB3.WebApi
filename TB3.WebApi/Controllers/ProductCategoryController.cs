using Microsoft.AspNetCore.Mvc;
using TB3.WebApi.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductCategoryController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductCategoryController()
    {
        _db = new AppDbContext();
    }

    // GET api/ProductCategory
    [HttpGet]
    public IActionResult GetCategories()
    {
        var lst = _db.TblProductCategories
            .OrderByDescending(x => x.ProductCategoryId)
            .ToList();

        return Ok(lst);
    }

    // GET api/ProductCategory/5
    [HttpGet("{id}")]
    public IActionResult GetCategory(int id)
    {
        var item = _db.TblProductCategories
            .FirstOrDefault(x => x.ProductCategoryId == id);

        if (item is null)
        {
            return NotFound("Category not found.");
        }

        var response = new ProductCategoryGetResponseDto
        {
            ProductCategoryCode = item.ProductCategoryCode,
            ProductCategoryName = item.ProductCategoryName
        };

        return Ok(response);
    }

    // POST api/ProductCategory
    [HttpPost]
    public IActionResult CreateCategory(ProductCategoryCreateRequestDto request)
    {
        _db.TblProductCategories.Add(new TblProductCategory
        {
            ProductCategoryCode = request.ProductCategoryCode,
            ProductCategoryName = request.ProductCategoryName
        });

        _db.SaveChanges();
        return Ok("Category Created.");
    }

    // PUT api/ProductCategory/5
    [HttpPut("{id}")]
    public IActionResult UpdateCategory(int id, ProductCategoryUpdateRequestDto request)
    {
        var item = _db.TblProductCategories
            .FirstOrDefault(x => x.ProductCategoryId == id);

        if (item is null)
        {
            return NotFound("Category not found.");
        }

        item.ProductCategoryCode = request.ProductCategoryCode;
        item.ProductCategoryName = request.ProductCategoryName;

        int result = _db.SaveChanges();
        string message = result > 0 ? "Updating Successful." : "Updating Failed.";

        return Ok(message);
    }

    // PATCH api/ProductCategory/5
    [HttpPatch("{id}")]
    public IActionResult PatchCategory(int id, ProductCategoryPatchRequestDto request)
    {
        var item = _db.TblProductCategories
            .FirstOrDefault(x => x.ProductCategoryId == id);

        if (item is null)
        {
            return NotFound("Category not found.");
        }

        if (!string.IsNullOrEmpty(request.ProductCategoryCode))
            item.ProductCategoryCode = request.ProductCategoryCode;

        if (!string.IsNullOrEmpty(request.ProductCategoryName))
            item.ProductCategoryName = request.ProductCategoryName;

        int result = _db.SaveChanges();
        string message = result > 0 ? "Patching Successful." : "Patching Failed.";

        return Ok(message);
    }

    
}

public class ProductCategoryUpdateRequestDto
{
    public string ProductCategoryCode { get; set; }
    public string ProductCategoryName { get; set; }
}

public class ProductCategoryCreateRequestDto
{
    public string ProductCategoryCode { get; set; }
    public string ProductCategoryName { get; set; }
}

public class ProductCategoryPatchRequestDto
{
    public string? ProductCategoryCode { get; set; }
    public string? ProductCategoryName { get; set; }
}

internal class ProductCategoryGetResponseDto
{
    public string ProductCategoryCode { get; set; }
    public string ProductCategoryName { get; set; }
}
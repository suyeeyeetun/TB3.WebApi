using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using TB3.WebApi.Database.AppDbContextModels;
using TB3.WebApi.Services;

namespace TB3.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{pageNo}/{pageSize}")]
    public IActionResult GetProducts(int pageNo, int pageSize)
    {
        var result = _productService.GetProducts(pageNo, pageSize);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var result = _productService.GetProductById(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost]
    public IActionResult CreateProducts(ProductCreateRequestDto request)
    {
        
        var result = _productService.CreateProducts(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    [HttpPut("{id}")]
    public IActionResult UpdateProducts(int id, ProductUpdateRequestDto request)
    {
        //var item = _db.TblProducts.FirstOrDefault(x => x.ProductId == id);
        //if (item is null)
        //{
        //    return NotFound("Product not found.");
        //}

        //item.ProductName = request.ProductName;
        //item.Price = request.Price;
        //item.Quantity = request.Quantity;
        //item.ModifiedDateTime = DateTime.Now;
        //int result = _db.SaveChanges();
        //string message = result > 0 ? "Updating Successful." : "Updating Failed.";

        //return Ok(message);
        var result = _productService.UpdateProducts(id,request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    [HttpPatch("{id}")]
    public IActionResult PatchProducts(int id, ProductPatchRequestDto request)
    {
        var result = _productService.PatchProducts(id, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);

    }
    [HttpDelete("{id}")]
    public IActionResult DeleteProducts(int id)
    {
        var result = _productService.DeleteProducts(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

public class ProductCreateRequestDto
{
    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}

public class ProductUpdateRequestDto
{
    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}
public class ProductPatchRequestDto
{
    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }
}

public class ProductGetResponseDto
{
    public string ProductName { get; set; }
    public bool IsSuccess { get; internal set; }
    public string Message { get; internal set; }
    public List<ProductDto> Products { get; internal set; }
}
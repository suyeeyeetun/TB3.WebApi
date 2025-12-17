using Microsoft.AspNetCore.Mvc;
using TB3.WebApi.Services;

namespace TB3.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductUsingAdoDotNetController : ControllerBase
{
    private readonly ProductAdoDotNetService _service;

    // ✅ Service injected via DI
    public ProductUsingAdoDotNetController(ProductAdoDotNetService service)
    {
        _service = service;
    }

    // **********************************************
    // GET PRODUCTS (PAGING)
    // **********************************************
    [HttpGet]
    public IActionResult GetProducts(int pageNo = 1, int pageSize = 10)
    {
        var result = _service.GetProducts(pageNo, pageSize);
        return Ok(result);
    }

    // **********************************************
    // GET PRODUCT BY ID
    // **********************************************
    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        var result = _service.GetProductById(id);
        return Ok(result);
    }

    // **********************************************
    // CREATE PRODUCT
    // **********************************************
    [HttpPost]
    public IActionResult CreateProduct([FromBody] ProductCreateRequestDto request)
    {
        var result = _service.CreateProducts(request);
        return Ok(result);
    }

    // **********************************************
    // UPDATE PRODUCT (PUT)
    // **********************************************
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateRequestDto request)
    {
        var result = _service.UpdateProducts(id, request);
        return Ok(result);
    }

    // **********************************************
    // PATCH PRODUCT
    // **********************************************
    [HttpPatch("{id}")]
    public IActionResult PatchProduct(int id, [FromBody] ProductPatchRequestDto request)
    {
        var result = _service.PatchProducts(id, request);
        return Ok(result);
    }

    // **********************************************
    // SOFT DELETE PRODUCT
    // **********************************************
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var result = _service.DeleteProducts(id);
        return Ok(result);
    }
}

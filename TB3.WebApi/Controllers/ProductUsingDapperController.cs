using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TB3.WebApi.Services;

namespace TB3.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductUsingDapperController : ControllerBase
{
    private readonly ProductDapperService _service;

    public ProductUsingDapperController(IConfiguration config)
    {
        _service = new ProductDapperService(
            config.GetConnectionString("DbConnection"));
    }

    [HttpGet]
    public IActionResult Get(int pageNo = 1, int pageSize = 10)
        => Ok(_service.GetProducts(pageNo, pageSize));

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
        => Ok(_service.GetProductById(id));

    [HttpPost]
    public IActionResult Create(ProductCreateRequestDto dto)
        => Ok(_service.CreateProducts(dto));

    [HttpPut("{id}")]
    public IActionResult Update(int id, ProductUpdateRequestDto dto)
        => Ok(_service.UpdateProducts(id, dto));

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, ProductPatchRequestDto dto)
        => Ok(_service.PatchProducts(id, dto));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
        => Ok(_service.DeleteProducts(id));
}

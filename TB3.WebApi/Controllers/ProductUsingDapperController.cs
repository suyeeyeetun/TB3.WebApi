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
    private readonly IProductDapperService _productDapperService;

    public ProductUsingDapperController(IProductDapperService productDapperService)
    {
        _productDapperService = productDapperService;
    }

    [HttpGet]
    public IActionResult Get(int pageNo = 1, int pageSize = 10)
        => Ok(_productDapperService.GetProducts(pageNo, pageSize));

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
        => Ok(_productDapperService.GetProductById(id));

    [HttpPost]
    public IActionResult Create(ProductCreateRequestDto dto)
        => Ok(_productDapperService.CreateProducts(dto));

    [HttpPut("{id}")]
    public IActionResult Update(int id, ProductUpdateRequestDto dto)
        => Ok(_productDapperService.UpdateProducts(id, dto));

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, ProductPatchRequestDto dto)
        => Ok(_productDapperService.PatchProducts(id, dto));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
        => Ok(_productDapperService.DeleteProducts(id));
}

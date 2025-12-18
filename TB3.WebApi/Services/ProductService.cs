using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using TB3.WebApi.Controllers;
using TB3.WebApi.Database.AppDbContextModels;

namespace TB3.WebApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db)
    {
        _db = db;
    }
    public ProductGetResponseDto GetProducts(int pageNo, int pageSize)
    {
        ProductGetResponseDto dto = new ProductGetResponseDto();
        if (pageNo == 0)
        {
            dto = new ProductGetResponseDto()
            {
                IsSuccess = false,
                Message = "Page size must be greater than zero."
            };
            return dto;
        }
        if (pageSize == 0)
        {
            dto = new ProductGetResponseDto()
            {
                IsSuccess = false,
                Message = "Page size must be greater than zero."
            };
            return dto;
        }
        var lst = _db.TblProducts
            .OrderByDescending(x => x.ProductId)
            .Where(x => x.DeleteFlag == false)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        //List<ProductDto> products = new List<ProductDto>();
        //foreach (var item in lst)
        //{
        //    ProductDto product = new ProductDto
        //    {
        //        ProductId = item.ProductId,
        //        ProductName = item.ProductName,
        //        Quantity = item.Quantity,
        //        Price = item.Price,
        //    };
        //    products.Add(product);
        //}
        var products = lst.Select(item => new ProductDto
        {
            Price = item.Price,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity
        }).ToList();
        dto = new ProductGetResponseDto()
        {
            IsSuccess = true,
            Message = "Success",
            Products = products
        };
        return dto;
    }
    public ProductGetByIdResponseDto GetProductById(int id)
    {
        ProductGetByIdResponseDto dto = new ProductGetByIdResponseDto();
        var item = _db.TblProducts
            .Where(x => x.DeleteFlag == false)
            .FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductGetByIdResponseDto()
            {
                IsSuccess = false,
                Message = "You must put the id."
            };
            return dto;
        }
        if (id <= 0)
        {
            dto = new ProductGetByIdResponseDto()
            {
                IsSuccess = false,
                Message = "Id must be greater than 0."
            };
            return dto;
        }
        var response = new ProductDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Price = item.Price,
            Quantity = item.Quantity,
        };
        dto = new ProductGetByIdResponseDto()
        {
            IsSuccess = true,
            Message = "Success",
            Product = response
        };
        return dto;
    }

    public ProductResponseDto CreateProducts(ProductCreateRequestDto request)
    {
        ProductResponseDto dto = new ProductResponseDto();

        if (string.IsNullOrEmpty(request.ProductName))
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Product name is required."
            };
            return dto;
        }

        if (request.Price == null && request.Price <= 0)
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Price must be greater than zero."
            };
            return dto;
        }

        if (request.Quantity == null && request.Quantity < 0)
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Quantity cannot be negative."
            };
            return dto;
        }
        _db.TblProducts.Add(new TblProduct
        {
            CreateDateTime = DateTime.Now,
            Price = request.Price,
            DeleteFlag = false,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
        });
        _db.SaveChanges();
        dto = new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Success"
        };
        return dto;
    }

    public ProductResponseDto UpdateProducts(int id, ProductUpdateRequestDto request)
    {
        ProductResponseDto dto = new ProductResponseDto();
        var item = _db.TblProducts
        .Where(x => x.DeleteFlag == false)
        .FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "You must put the id to update."
            };
            return dto;
        }
        if (string.IsNullOrEmpty(request.ProductName))
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Product name is required."
            };
            return dto;
        }

        if (request.Price == null && request.Price <= 0)
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Price must be greater than zero."
            };
            return dto;
        }

        if (request.Quantity == null && request.Quantity < 0)
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "Quantity cannot be negative."
            };
            return dto;
        }

        item.ProductName = request.ProductName;
        item.Price = request.Price;
        item.Quantity = request.Quantity;
        item.ModifiedDateTime = DateTime.Now;
        _db.SaveChanges();
        dto = new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Success"
        };
        return dto;
    }

    public ProductResponseDto PatchProducts(int id, ProductPatchRequestDto request)
    {
        ProductResponseDto dto = new ProductResponseDto();

        var item = _db.TblProducts
        .Where(x => x.DeleteFlag == false)
        .FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "You must put the id to update."
            };
            return dto;
        }

        if (string.IsNullOrEmpty(request.ProductName) && (request.Price <= 0 || request.Price is null) && (request.Quantity <= 0 || request.Quantity is null))
        {
            dto = new ProductResponseDto
            {
                IsSuccess = false,
                Message = "You need to put at least one value."
            };
            return dto;
        }
        if (!string.IsNullOrEmpty(request.ProductName))
            item.ProductName = request.ProductName;

        if (request.Price is not null && request.Price > 0)
            item.Price = request.Price ?? 0;

        if (request.Quantity is not null && request.Quantity > 0)
            item.Quantity = request.Quantity ?? 0;

        item.ModifiedDateTime = DateTime.Now;
        int result = _db.SaveChanges();
        dto = new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Success"
        };
        return dto;
    }

    public ProductResponseDto DeleteProducts(int id)
    {
        ProductResponseDto dto = new ProductResponseDto();
        var item = _db.TblProducts
            .Where(x => x.DeleteFlag == false)
            .FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "You must put the id to delete."
            };
            return dto;
        }
        if (id <= 0)
        {
            dto = new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Id must be greater than 0."
            };
            return dto;
        }
        item.DeleteFlag = true;
        _db.SaveChanges();
        dto = new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Success"
        };
        return dto;
    }
}

public class ProductGetResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<ProductDto> Products { get; set; }
}
public class ProductResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}

public class ProductGetByIdResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public ProductDto Product { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}


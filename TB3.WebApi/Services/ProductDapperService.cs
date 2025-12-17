using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TB3.WebApi.Controllers;

namespace TB3.WebApi.Services
{
    public class ProductDapperService
    {
        private readonly string _connectionString;

        public ProductDapperService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // **********************************************
        // GET PRODUCTS (PAGING)
        // **********************************************
        public ProductGetResponseDto GetProducts(int pageNo, int pageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return new ProductGetResponseDto
                {
                    IsSuccess = false,
                    Message = "Page number and page size must be greater than zero."
                };
            }

            using var db = GetConnection();

            string query = @"
            SELECT ProductId, ProductName, Quantity, Price
            FROM Tbl_Product
            WHERE DeleteFlag = 0
            ORDER BY ProductId DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var products = db.Query<ProductDto>(query, new
            {
                Offset = (pageNo - 1) * pageSize,
                PageSize = pageSize
            }).ToList();

            return new ProductGetResponseDto
            {
                IsSuccess = true,
                Message = "Success",
                Products = products
            };
        }

        // **********************************************
        // GET PRODUCT BY ID
        // **********************************************
        public ProductGetByIdResponseDto GetProductById(int id)
        {
            if (id <= 0)
            {
                return new ProductGetByIdResponseDto
                {
                    IsSuccess = false,
                    Message = "Id must be greater than 0."
                };
            }

            using var db = GetConnection();

            string query = @"
            SELECT ProductId, ProductName, Quantity, Price
            FROM Tbl_Product
            WHERE ProductId = @Id AND DeleteFlag = 0";

            var product = db.QueryFirstOrDefault<ProductDto>(query, new { Id = id });

            if (product == null)
            {
                return new ProductGetByIdResponseDto
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }

            return new ProductGetByIdResponseDto
            {
                IsSuccess = true,
                Message = "Success",
                Product = product
            };
        }

        // **********************************************
        // CREATE PRODUCT
        // **********************************************
        public ProductResponseDto CreateProducts(ProductCreateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
                return Fail("Product name is required.");

            if (request.Price <= 0)
                return Fail("Price must be greater than zero.");

            if (request.Quantity < 0)
                return Fail("Quantity cannot be negative.");

            using var db = GetConnection();

            string query = @"
            INSERT INTO Tbl_Product
            (ProductName, Quantity, Price, DeleteFlag, CreateDateTime)
            VALUES (@ProductName, @Quantity, @Price, 0, GETDATE())";

            int result = db.Execute(query, request);

            return result > 0 ? Success() : Fail("Create failed.");
        }

        // **********************************************
        // UPDATE PRODUCT (PUT)
        // **********************************************
        public ProductResponseDto UpdateProducts(int id, ProductUpdateRequestDto request)
        {
            if (id <= 0)
                return Fail("Id must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.ProductName))
                return Fail("Product name is required.");

            if (request.Price <= 0)
                return Fail("Price must be greater than zero.");

            if (request.Quantity < 0)
                return Fail("Quantity cannot be negative.");

            using var db = GetConnection();

            string query = @"
            UPDATE Tbl_Product
            SET ProductName=@ProductName,
                Quantity=@Quantity,
                Price=@Price,
                ModifiedDateTime=GETDATE()
            WHERE ProductId=@Id AND DeleteFlag = 0";

            int result = db.Execute(query, new
            {
                Id = id,
                request.ProductName,
                request.Quantity,
                request.Price
            });

            return result > 0 ? Success() : Fail("Update failed.");
        }

        // **********************************************
        // PATCH PRODUCT
        // **********************************************
        public ProductResponseDto PatchProducts(int id, ProductPatchRequestDto request)
        {
            using var db = GetConnection();

            string selectQuery = @"
            SELECT ProductId, ProductName, Quantity, Price
            FROM Tbl_Product
            WHERE ProductId=@Id AND DeleteFlag = 0";

            var product = db.QueryFirstOrDefault<ProductDto>(selectQuery, new { Id = id });

            if (product == null)
                return Fail("Product not found.");

            if (string.IsNullOrEmpty(request.ProductName)
                && request.Price == null
                && request.Quantity == null)
            {
                return Fail("You need to put at least one value.");
            }

            product.ProductName = request.ProductName ?? product.ProductName;
            product.Price = request.Price ?? product.Price;
            product.Quantity = request.Quantity ?? product.Quantity;

            string updateQuery = @"
            UPDATE Tbl_Product
            SET ProductName=@ProductName,
                Quantity=@Quantity,
                Price=@Price,
                ModifiedDateTime=GETDATE()
            WHERE ProductId=@ProductId";

            int result = db.Execute(updateQuery, product);

            return result > 0 ? Success() : Fail("Patch failed.");
        }

        // **********************************************
        // SOFT DELETE
        // **********************************************
        public ProductResponseDto DeleteProducts(int id)
        {
            if (id <= 0)
                return Fail("Id must be greater than zero.");

            using var db = GetConnection();

            string query = @"
            UPDATE Tbl_Product
            SET DeleteFlag = 1,
                ModifiedDateTime = GETDATE()
            WHERE ProductId=@Id AND DeleteFlag = 0";

            int result = db.Execute(query, new { Id = id });

            return result > 0 ? Success() : Fail("Delete failed.");
        }

        // **********************************************
        // HELPER METHODS
        // **********************************************
        private ProductResponseDto Success() =>
            new ProductResponseDto { IsSuccess = true, Message = "Success" };

        private ProductResponseDto Fail(string message) =>
            new ProductResponseDto { IsSuccess = false, Message = message };
    }
}
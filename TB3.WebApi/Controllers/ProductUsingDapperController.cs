using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductUsingDapperController : ControllerBase
    {
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = ".", // change if needed
            InitialCatalog = "Batch3MiniPOS", // change to your DB
            UserID = "sa",
            Password = "sasa@123",
            TrustServerCertificate = true
        };

        private IDbConnection GetConnection()
        {
            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }

        // **********************************************
        // GET ALL PRODUCTS
        // **********************************************
        [HttpGet]
        public IActionResult GetProducts()
        {
            using (IDbConnection db = GetConnection())
            {
                string query = "SELECT * FROM Tbl_Product WHERE DeleteFlag = 0 ORDER BY ProductId DESC";
                var products = db.Query<dynamic>(query).ToList();
                return Ok(products);
            }
        }

        // **********************************************
        // GET PRODUCT BY ID
        // **********************************************
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            using (IDbConnection db = GetConnection())
            {
                string query = "SELECT * FROM Tbl_Product WHERE ProductId=@Id AND DeleteFlag = 0";
                var product = db.QueryFirstOrDefault<dynamic>(query, new { Id = id });

                if (product == null)
                    return NotFound("Product not found.");

                return Ok(product);
            }
        }

        // **********************************************
        // CREATE PRODUCT
        // **********************************************
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductCreateDto request)
        {
            using (IDbConnection db = GetConnection())
            {
                string query = @"
                    INSERT INTO Tbl_Product
                    (ProductName, Quantity, Price, DeleteFlag, CreateDateTime)
                    VALUES (@ProductName, @Quantity, @Price, 0, GETDATE())";

                int result = db.Execute(query, request);
                return result > 0 ? Ok("Product created.") : BadRequest("Create failed.");
            }
        }

        // **********************************************
        // UPDATE PRODUCT
        // **********************************************
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateDto request)
        {
            using (IDbConnection db = GetConnection())
            {
                string query = @"
                    UPDATE Tbl_Product
                    SET ProductName=@ProductName,
                        Quantity=@Quantity,
                        Price=@Price,
                        ModifiedDateTime=GETDATE()
                    WHERE ProductId=@ProductId";

                int result = db.Execute(query, new
                {
                    ProductId = id,
                    request.ProductName,
                    request.Quantity,
                    request.Price
                });

                return result > 0 ? Ok("Product updated.") : BadRequest("Update failed.");
            }
        }

        // **********************************************
        // PATCH PRODUCT
        // **********************************************
        [HttpPatch("{id}")]
        public IActionResult PatchProduct(int id, [FromBody] ProductPatchDto request)
        {
            using (IDbConnection db = GetConnection())
            {
                string selectQuery = "SELECT * FROM Tbl_Product WHERE ProductId=@Id AND DeleteFlag = 0";
                var product = db.QueryFirstOrDefault<dynamic>(selectQuery, new { Id = id });
                if (product == null) return NotFound("Product not found.");

                product.ProductName = !string.IsNullOrEmpty(request.ProductName) ? request.ProductName : product.ProductName;
                product.Quantity = (request.Quantity.HasValue && request.Quantity.Value > 0) ? request.Quantity.Value : product.Quantity;
                product.Price = (request.Price.HasValue && request.Price.Value > 0) ? request.Price.Value : product.Price;

                string updateQuery = @"
                    UPDATE Tbl_Product
                    SET ProductName=@ProductName,
                        Quantity=@Quantity,
                        Price=@Price,
                        ModifiedDateTime=GETDATE()
                    WHERE ProductId=@ProductId";

                int result = db.Execute(updateQuery, new
                {
                    ProductId = id,
                    ProductName = product.ProductName,
                    Quantity = product.Quantity,
                    Price = product.Price
                });

                return result > 0 ? Ok("Product patched.") : BadRequest("Patch failed.");
            }
        }

        // **********************************************
        // SOFT DELETE PRODUCT
        // **********************************************
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            using (IDbConnection db = GetConnection())
            {
                string query = @"
                    UPDATE Tbl_Product
                    SET DeleteFlag=1, ModifiedDateTime=GETDATE()
                    WHERE ProductId=@ProductId";

                int result = db.Execute(query, new { ProductId = id });
                return result > 0 ? Ok("Product deleted.") : BadRequest("Delete failed.");
            }
        }

        // **********************************************
        // DTO CLASSES
        // **********************************************
        public class ProductCreateDto
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class ProductUpdateDto
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class ProductPatchDto
        {
            public string? ProductName { get; set; }
            public int? Quantity { get; set; }
            public decimal? Price { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductUsingAdoDotNetController : ControllerBase
    {
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = ".",
            InitialCatalog = "Batch3MiniPOS",
            UserID = "sa",
            Password = "sasa@123",
            TrustServerCertificate = true
        };

        private SqlConnection GetConnection()
        {
            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }

        // **********************************************
        // GET ALL PRODUCTS
        // **********************************************
        [HttpGet]
        public IActionResult GetProducts()
        {
            List<dynamic> list = new();

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Tbl_Product WHERE DeleteFlag = 0 ORDER BY ProductId DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new
                    {
                        ProductId = reader["ProductId"],
                        ProductName = reader["ProductName"],
                        Quantity = reader["Quantity"],
                        Price = reader["Price"],
                        DeleteFlag = reader["DeleteFlag"],
                        CreateDateTime = reader["CreateDateTime"],
                        ModifiedDateTime = reader["ModifiedDateTime"]
                    });
                }
            }

            return Ok(list);
        }

        // **********************************************
        // GET PRODUCT BY ID
        // **********************************************
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            dynamic? product = null;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Tbl_Product WHERE ProductId=@Id AND DeleteFlag=0";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    product = new
                    {
                        ProductId = reader["ProductId"],
                        ProductName = reader["ProductName"],
                        Quantity = reader["Quantity"],
                        Price = reader["Price"]
                    };
                }
            }

            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // **********************************************
        // CREATE PRODUCT
        // **********************************************
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductCreateReqDto request)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Tbl_Product
                                 (ProductName, Quantity, Price, DeleteFlag, CreateDateTime)
                                 VALUES (@Name, @Qty, @Price, 0, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", request.ProductName);
                cmd.Parameters.AddWithValue("@Qty", request.Quantity);
                cmd.Parameters.AddWithValue("@Price", request.Price);

                int result = cmd.ExecuteNonQuery();
                return result > 0 ? Ok("Product created.") : BadRequest("Create failed.");
            }
        }

        // **********************************************
        // UPDATE PRODUCT
        // **********************************************
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateReqDto request)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Tbl_Product
                                 SET ProductName=@Name,
                                     Quantity=@Qty,
                                     Price=@Price,
                                     ModifiedDateTime=GETDATE()
                                 WHERE ProductId=@Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", request.ProductName);
                cmd.Parameters.AddWithValue("@Qty", request.Quantity);
                cmd.Parameters.AddWithValue("@Price", request.Price);

                int result = cmd.ExecuteNonQuery();
                return result > 0 ? Ok("Product updated.") : BadRequest("Update failed.");
            }
        }

        // **********************************************
        // PATCH PRODUCT
        // **********************************************
        [HttpPatch("{id}")]
        public IActionResult PatchProduct(int id, [FromBody] ProductPatchReqDto request)
        {
            string? name = null;
            int? qty = null;
            decimal? price = null;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                // Load existing product
                string selectQuery = "SELECT ProductName, Quantity, Price FROM Tbl_Product WHERE ProductId=@Id AND DeleteFlag=0";
                SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
                selectCmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = selectCmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("Product not found.");

                name = reader["ProductName"].ToString();
                qty = Convert.ToInt32(reader["Quantity"]);
                price = Convert.ToDecimal(reader["Price"]);

                reader.Close();

                // Apply patch values
                string finalName = request.ProductName ?? name!;
                int finalQty = request.Quantity ?? qty.Value;
                decimal finalPrice = request.Price ?? price.Value;

                string updateQuery = @"UPDATE Tbl_Product
                                       SET ProductName=@Name,
                                           Quantity=@Qty,
                                           Price=@Price,
                                           ModifiedDateTime=GETDATE()
                                       WHERE ProductId=@Id";

                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Id", id);
                updateCmd.Parameters.AddWithValue("@Name", finalName);
                updateCmd.Parameters.AddWithValue("@Qty", finalQty);
                updateCmd.Parameters.AddWithValue("@Price", finalPrice);

                int result = updateCmd.ExecuteNonQuery();
                return result > 0 ? Ok("Product patched.") : BadRequest("Patch failed.");
            }
        }

        // **********************************************
        // SOFT DELETE PRODUCT
        // **********************************************
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Tbl_Product
                                 SET DeleteFlag=1, ModifiedDateTime=GETDATE()
                                 WHERE ProductId=@Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                int result = cmd.ExecuteNonQuery();
                return result > 0 ? Ok("Product deleted.") : BadRequest("Delete failed.");
            }
        }

        // **********************************************
        // DTO CLASSES
        // **********************************************
        public class ProductCreateReqDto
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class ProductUpdateReqDto
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class ProductPatchReqDto
        {
            public string? ProductName { get; set; }
            public int? Quantity { get; set; }
            public decimal? Price { get; set; }
        }
    }
}

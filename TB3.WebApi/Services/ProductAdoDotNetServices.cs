using Microsoft.Data.SqlClient;
using System.Data;
using TB3.WebApi.Controllers;

namespace TB3.WebApi.Services
{
    public class ProductAdoDotNetService : IProductAdoDotNetService
    {
        private readonly string _connectionString;

        public ProductAdoDotNetService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnection")!;
        }


        // GET PRODUCTS (PAGING)
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

            List<ProductDto> products = new();

            SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();

                string query = @"
                    SELECT ProductId, ProductName, Quantity, Price
                    FROM Tbl_Product
                    WHERE DeleteFlag = 0
                    ORDER BY ProductId DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Offset", (pageNo - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new ProductDto
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductName = reader["ProductName"].ToString()!,
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    });
                }
            conn.Close();

            return new ProductGetResponseDto
            {
                IsSuccess = true,
                Message = "Success",
                Products = products
            };
        }

        // GET PRODUCT BY ID
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

            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                SELECT ProductId, ProductName, Quantity, Price
                FROM Tbl_Product
                WHERE ProductId=@Id AND DeleteFlag=0";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            
            if (!reader.Read())
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
                Product = new ProductDto
                {
                    ProductId = Convert.ToInt32(reader["ProductId"]),
                    ProductName = reader["ProductName"].ToString()!,
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["Price"])
                }
            };
            conn.Close();
        }

        // CREATE PRODUCT
        public ProductResponseDto CreateProducts(ProductCreateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
                return Fail("Product name is required.");

            if (request.Price <= 0)
                return Fail("Price must be greater than zero.");

            if (request.Quantity < 0)
                return Fail("Quantity cannot be negative.");

            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Tbl_Product
                (ProductName, Quantity, Price, DeleteFlag, CreateDateTime)
                VALUES (@Name, @Qty, @Price, 0, GETDATE())";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", request.ProductName);
            cmd.Parameters.AddWithValue("@Qty", request.Quantity);
            cmd.Parameters.AddWithValue("@Price", request.Price);
           
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0 ? Success() : Fail("Create failed.");
        }

       
        // UPDATE PRODUCT (PUT)
        public ProductResponseDto UpdateProducts(int id, ProductUpdateRequestDto request)
        {
            if (id <= 0)
                return Fail("Id must be greater than zero.");

            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                UPDATE Tbl_Product
                SET ProductName=@Name,
                    Quantity=@Qty,
                    Price=@Price,
                    ModifiedDateTime=GETDATE()
                WHERE ProductId=@Id AND DeleteFlag=0";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", request.ProductName);
            cmd.Parameters.AddWithValue("@Qty", request.Quantity);
            cmd.Parameters.AddWithValue("@Price", request.Price);
            
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0 ? Success() : Fail("Update failed.");
        }

        // PATCH PRODUCT
        public ProductResponseDto PatchProducts(int id, ProductPatchRequestDto request)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string selectQuery = @"
                SELECT ProductName, Quantity, Price
                FROM Tbl_Product
                WHERE ProductId=@Id AND DeleteFlag=0";

            SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = selectCmd.ExecuteReader();
            if (!reader.Read())
                return Fail("Product not found.");

            string name = reader["ProductName"].ToString()!;
            int qty = Convert.ToInt32(reader["Quantity"]);
            decimal price = Convert.ToDecimal(reader["Price"]);
            reader.Close();

            name = request.ProductName ?? name;
            qty = request.Quantity ?? qty;
            price = request.Price ?? price;

            string updateQuery = @"
                UPDATE Tbl_Product
                SET ProductName=@Name,
                    Quantity=@Qty,
                    Price=@Price,
                    ModifiedDateTime=GETDATE()
                WHERE ProductId=@Id";

            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.Parameters.AddWithValue("@Name", name);
            updateCmd.Parameters.AddWithValue("@Qty", qty);
            updateCmd.Parameters.AddWithValue("@Price", price);
           
            int result = updateCmd.ExecuteNonQuery();
            conn.Close();
            return result > 0 ? Success() : Fail("Patch failed.");
        }

        // SOFT DELETE
        public ProductResponseDto DeleteProducts(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                UPDATE Tbl_Product
                SET DeleteFlag=1, ModifiedDateTime=GETDATE()
                WHERE ProductId=@Id AND DeleteFlag=0";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
           
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0 ? Success() : Fail("Delete failed.");
        }

        // Dtos
        private ProductResponseDto Success()
            => new ProductResponseDto { IsSuccess = true, Message = "Success" };

        private ProductResponseDto Fail(string message)
            => new ProductResponseDto { IsSuccess = false, Message = message };
    }
}

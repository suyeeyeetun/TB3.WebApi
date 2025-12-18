using TB3.WebApi.Controllers;

namespace TB3.WebApi.Services
{
    public interface IProductDapperService
    {
        ProductResponseDto CreateProducts(ProductCreateRequestDto request);
        ProductResponseDto DeleteProducts(int id);
        ProductGetByIdResponseDto GetProductById(int id);
        ProductGetResponseDto GetProducts(int pageNo, int pageSize);
        ProductResponseDto PatchProducts(int id, ProductPatchRequestDto request);
        ProductResponseDto UpdateProducts(int id, ProductUpdateRequestDto request);
    }
}
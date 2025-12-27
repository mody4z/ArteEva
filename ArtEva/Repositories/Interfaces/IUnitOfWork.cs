using ArteEva.Repositories;

namespace ArtEva.Repositories.Interfaces
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        IShopRepository ShopRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        ICartRepository CartRepository { get; }
        IAddressRepository AddressRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        Task<int> SaveChangesAsync();
    }
}

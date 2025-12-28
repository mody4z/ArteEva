using ArteEva.Models;

public class CartDomainService
{
    public CartItem AddItem(Cart cart, int productId,string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        var item = cart.CartItems
            .FirstOrDefault(x => x.ProductId == productId && !x.IsDeleted);

        if (item != null)
        {
            item.Quantity += quantity;
            item.TotalPrice = item.Quantity * item.UnitPrice;
            return item;
        }

        item = new CartItem
        {
             CartId = cart.Id,
            ProductName = productName, // ✅ مهم

            UserId = cart.UserId,          // ✅ أهم سطر ناقص عندك

            ProductId = productId,
            Quantity = quantity,
            UnitPrice = price,
            TotalPrice = price * quantity,
            IsDeleted = false
        };

        cart.CartItems.Add(item);

        return item;
    }
    public CartItem UpdateQuantity(Cart cart, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        var item = cart.CartItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            throw new Exception("Item not found in cart");

        item.Quantity = quantity;
        item.TotalPrice = item.UnitPrice * quantity;

        return item;
    }

    public CartItem RemoveItem(Cart cart, int productId)
    {
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            throw new Exception("Item not found");

        cart.CartItems.Remove(item);
        return item;
    }
}

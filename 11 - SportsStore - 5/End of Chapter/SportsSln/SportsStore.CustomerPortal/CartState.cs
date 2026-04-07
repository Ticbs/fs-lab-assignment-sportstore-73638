using SportsStore.CustomerPortal.Services;

namespace SportsStore.CustomerPortal;

public static class CartState
{
    public static List<CartItem> Items { get; set; } = new();
}

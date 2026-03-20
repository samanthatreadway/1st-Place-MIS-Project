using System;
using System.Collections.Generic;
using System.Linq;
using fa25group23final.Models;

namespace fa25group23final.Models.ViewModels.Orders
{
    public class ShoppingCartViewModel
    {
        // The current shopping cart order
        public List<OrderDetails> Items { get; set; } = new List<OrderDetails>();

        // One-time message to display when cart is empty or on other statuses
        public List<string> Messages { get; set; } = new List<string>();

        // Computed read-only helpers
        public int ItemCount => Items?.Sum(i => i.Quantity) ?? 0;
        public decimal Subtotal => Items?.Sum(i => i.Price * i.Quantity) ?? 0m;

        // Calculate shipping and total on demand using explicit rates.
        // Keep rates external so checkout and admin settings are the single source of truth.
        public(decimal Shipping, decimal Total) CalculateTotals(decimal firstItemShipping, decimal additionalItemShipping)
        {
            var qty = ItemCount;
            decimal shipping = 0m;
            if (qty > 0)
            {
                shipping = qty == 1
                    ? firstItemShipping
                    : firstItemShipping + (qty - 1) * additionalItemShipping;
            }

            return (shipping, Subtotal + shipping);
        }

        // Convenience overload that accepts a Shippingconfig entity.
        public (decimal Shipping, decimal Total) CalculateTotals(Shippingconfig config)
        {
            if (config == null)
            {
                // fallback to sensible defaults if config is missing
                return CalculateTotals(3.50m, 1.50m);
            }

            return CalculateTotals(config.FirstBookFee, config.AdditionalBookFee);
        }
    }
}

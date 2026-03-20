using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models.ViewModels.Orders
{
    /// <summary>
    /// CheckoutViewModel extends ShoppingCartViewModel to add checkout-specific functionality
    /// - Reuses Items, Messages, ItemCount, Subtotal from parent
    /// - Reuses CalculateTotals() method for base shipping calculation
    /// - Adds checkout-specific properties (credit cards, coupons)
    /// </summary>
    public class CheckoutViewModel : ShoppingCartViewModel
    {
        // Credit card selection
        public List<SelectListItem> CreditCardOptions { get; set; } = new List<SelectListItem>();
        
        [Required(ErrorMessage = "Please select a credit card")]
        [Display(Name = "Select Credit Card")]
        public int? SelectedCreditCardId { get; set; }
        
        // Coupon code - NULLABLE because it's optional
        [Display(Name = "Coupon Code (Optional)")]
        [RegularExpression("^[A-Za-z0-9]{1,20}$", ErrorMessage = "Invalid coupon code format")]
        public string? CouponCode { get; set; }

        public Coupon AppliedCoupon { get; set; }
        
        // Shipping configuration
        public Shippingconfig ShippingConfig { get; set; }
        
        // Calculated values for checkout display
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        
        // Shipping address - from AppUser
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        // Future-proof: When address is separated into components
        // These properties will be populated from AppUser.Street, AppUser.City, etc.
        // For now, they're all null and we use ShippingAddress
        [Display(Name = "Street Address")]
        public string? Street { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }

        // Helper property to check if address is separated or combined
        public bool IsAddressSeparated => !string.IsNullOrEmpty(Street) &&
                                         !string.IsNullOrEmpty(City) &&
                                         !string.IsNullOrEmpty(State) &&
                                         !string.IsNullOrEmpty(ZipCode);

        /// <summary>
        /// Calculate checkout totals including shipping and coupon discounts
        /// Uses the parent class's CalculateTotals method and adds coupon logic
        /// </summary>
        //public void CalculateCheckoutTotals()
        //{
        //    // Ensure we have shipping config
        //    if (ShippingConfig == null)
        //    {
        //        ShippingConfig = new Shippingconfig
        //        {
        //            FirstBookFee = 3.50m,
        //            AdditionalBookFee = 1.50m
        //        };
        //    }

        //    // Use parent class method to calculate base shipping
        //    // This reuses the logic from ShoppingCartViewModel
        //    var baseTotals = CalculateTotals(ShippingConfig);
        //    ShippingFee = baseTotals.Shipping;

        //    // Calculate discount amount based on applied coupon
        //    DiscountAmount = 0m;

        //    if (AppliedCoupon != null)
        //    {
        //        if (AppliedCoupon.CouponType == CouponTypeEnum.Discount && AppliedCoupon.DiscountPercent.HasValue)
        //        {
        //            // Percentage discount off subtotal
        //            DiscountAmount = Subtotal * (AppliedCoupon.DiscountPercent.Value / 100m);
        //        }
        //        else if (AppliedCoupon.CouponType == CouponTypeEnum.FreeShipping)
        //        {
        //            // Free shipping if threshold is met (or no threshold exists)
        //            if (!AppliedCoupon.FreeThreshold.HasValue || Subtotal >= AppliedCoupon.FreeThreshold.Value)
        //            {
        //                // Discount equals the shipping fee (making it free)
        //                DiscountAmount = ShippingFee;
        //            }
        //        }
        //    }

        //    // Calculate final total: Subtotal + Shipping - Discount
        //    Total = Subtotal + ShippingFee - DiscountAmount;

        //    // Ensure total is never negative
        //    if (Total < 0)
        //    {
        //        Total = 0;
        //    }
        //}
    }
}

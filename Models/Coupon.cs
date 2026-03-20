using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{

	public enum CouponTypeEnum
	{
		[Display(Name = "Free Shipping Coupon")]
		FreeShipping,
		[Display(Name = "Discount Coupon")]
		Discount
	}

	public class Coupon
	{

        //Primary Key
        [Key]
		public int CouponID { get; set; }


        //Scalar Properties
        [RegularExpression("^[A-Za-z0-9]{1,20}$",
			ErrorMessage = "Code must be 1–20 characters and contain only letters and numbers.")]
        public string CouponCode { get; set; } //Coupon Code should be actual code the user inputs to activate coupon


		// There are two types of coupons: Free shipping for orders above $X. The admin also has the option to give free shipping for all orders
		// OR X% off your total order
		//I feel like this could be represented as a enum or boolean that indicates what type of coupon it is
		//Lets choose enum
		[Display(Name = "Coupon Type:")]
        public CouponTypeEnum CouponType { get; set; }

		//Nullable bc not all coupons will be discount % based
		[Display(Name = "Percent Off:")]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal? DiscountPercent { get; set; } // Value of how much the coupon is worth // discount amount

		// This property represents the minimum order amount required to qualify for free shipping
		// FOR FREE SHIPPING COUPON TYPES ONLY
		[Display(Name = "Minimum Amount for Free Shipping:")]
        [Range(0, double.MaxValue, ErrorMessage = "Must be 0 or greater")]
        public decimal? FreeThreshold { get; set; } // why is not and decimal? I CHANGED THIS TO DECIMAL FROM STRING OWEN

		[Display(Name = "Coupon Active:")]
		public bool Status { get; set; } //Status represents whether the coupon is active or not, AKA has it been used yet

		//Navigational Properties

		public List<Orders>? orders { get; set; }
        //1 to many (optional both ways) ; a coupon can have many orders, and a order can have 1 coupon


    }
}


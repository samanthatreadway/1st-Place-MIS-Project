using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
	public class Orders
	{

		//Primary Key
        [Key]
		public int OrderID { get; set; }

		//Scalar Properties
		[Display(Name = "Order Date:")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? OrderDate { get; set; } //Date when order is placed, later any user should not be able to edit this

		// Directly from the spec: This system does not keep track of the shipping status of an item – assume that an item is shipped and
		// delivered as soon as the order is placed.
		// Wait but I think this property is to track the customer's shopping cart. So while it is sitting in their shopping cart, this value will be false. 
		// Because you need to save the customer's shopping cart before they have checked out
		[Display(Name = "Order Status:")]
        public bool OrderStatus { get; set; } //True = Customer has Checked out, False = Still in shopping cart

        // these properties are stored here for record-keeping
        [Display(Name = "Shipping Fee:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? ShippingFee { get; set; }
        // I dont think admin would have a shipping addy?

        [Display(Name = "Shipping Address:")]
        public string? ShippingAddress { get; set; }

        //Navigational Properties
        //Customer Class
        public AppUser customerID { get; set; }

        //Card Class
        // Probably not nullable because an order should always have a credit card associated with it
		// not unless its still in the shopping cart lol
        public CreditCard? creditCard { get; set; }

		//Coupon Class
		public Coupon? couponID { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
            = new List<OrderDetails>();
    }
}


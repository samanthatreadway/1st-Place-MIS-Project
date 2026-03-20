using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
	//Define Enum for Card Type
	public enum CreditCardTypeEnum
	{
		[Display(Name = "Discover")]
		Discover,
        [Display(Name = "Visa")]
        Visa,
        [Display(Name = "Master Card")]
        Mastercard,
		[Display(Name = "American Express")]
		Amex
	}


	public class CreditCard
	{
        /* Credit Card class holds all information regarding customer credit cards
		 * Only Customers should need access to the class, as employees/admins should not be able
		 * to checkout/use cards
		*/

        //Primary Key CardID
        [Key]
		public int CardID { get; set; }

        //Scalar Properties
        // annotations to make the max length and only include numerical values
        // It doesn't matter too much i think for migrations and DB but good to have
        //Actually the annotations can mess up like model state stuff later on so idk
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Credit card number must be exactly 16 digits.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card number must be 16 digits and contain numbers only.")]
		[Display(Name = "Credit Card Number:")]
        public string CardNumber { get; set; }

		[Display(Name = "Card Type:")]
		public CreditCardTypeEnum CardType { get; set; }

		//Navigational Property
		//This is the many side so each card should only have 1 user
		public AppUser CustomerID { get; set; }

		public List<Orders>? orders { get; set; }
		// 1 to many relationship
		//Credit cards can be linked to certain orders, they can have many orders, while order has 1 credit card


	}
}


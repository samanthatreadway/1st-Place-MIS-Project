using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace fa25group23final.Models
{
	public class AppUser : IdentityUser
	{
		/* TESTTT
		 * AppUser Class inherits from Identity User, many properties are inhereted as well
		 */

		//INHERETIED PROPERTIES:
		/*
		 * UserID - Inherited Primary Key from Identity
		 * Username
		 * Email
		 *** Password 
		 * Phone
		 */

        //Non-inherited properties from IdentityUser class
        public string FirstName { get; set; }
		public string LastName { get; set; }

		public string Address { get; set; } //Address will be one singular string combined zip, state, etc. (Can be separate if you wanted)

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Display(Name = "Phone Number")]
        public override string PhoneNumber { get; set; }

        public bool Status { get; set; } //Status reflects if the user is active; Not Roles, we will define roles elsewhere

		//NAVIGATIONAL PROPERTIES

		//SPECIAL: the review class has 2 properties a reviewer and employee approver, in user class we should need just 1 property, due to
		// reviewer/approver is based on role authentication
		public List<Reviews>? ApprovedReviews { get; set; } = new List<Reviews>();
		public List<Reviews>? WrittenReviews { get; set; } = new List<Reviews>();
        //User - Review Connection, 1- (optional) many ; a user can have many reviews, while a review is tied to 1 user

        //annotation should set credit cards between 1-3
        [CreditCardCount(0, 3)]
        public List<CreditCard> AppUserCreditCardList { get; set; } = new List<CreditCard>();
		//User-CreditCard connection, 1 to many (non optional) -- credit cards are between range of 1-3

		public List<Orders>? AppUserOrdersList { get; set; }
        //User-Orders connection, 1 to (optional) many -- a user can have many orders, but an order is associated with a single user

    }

    // this should limit the credit cards to 3 max
    public class CreditCardCountAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        public CreditCardCountAttribute(int min, int max)
        {
            _min = min;   // usually 0
            _max = max;   // usually 3
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as ICollection<CreditCard>;
            int count = list?.Count ?? 0;

            if (count >= _min && count <= _max)
            {
                return ValidationResult.Success;
            }

            // Friendly, correct message
            if (_min == 0)
                return new ValidationResult($"User may have up to {_max} credit cards.");
            else
                return new ValidationResult($"User must have between {_min} and {_max} credit cards.");
        }
    }
}


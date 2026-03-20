using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
	public class OrderDetails
	{

		/*
		 * OrderDetails model class is a bridge table modell class inbetween Books Class and Orders Class
		 * we include bridge table to help carry payload of: Price, Cost, Quantity
		 */


		[Key]
		public int OrderDetailsID { get; set; } //Primary Key for the Brdige Table

		//Payload Properties / Scalar Properties
		[Display(Name = "Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; } //TODO: Document both price and cost and what they represent

		[Display(Name = "Cost:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Cost { get; set; }

		[Display(Name = "Quantity:")]
		public int Quantity { get; set; } //Quantity of the single book

        //Navigational Properties

        //OrderID
        public Orders orderID { get; set; } //OrderDetails is many side, therefore it connects to 1 order

        //BookID
        public Books bookID { get; set; } //OrderDetails is many side, therefore connects to single book

	}
}


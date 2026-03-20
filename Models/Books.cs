using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fa25group23final.Models
{
	public class Books
	{

		[Key]
		public int BookID { get; set; } //PK for book class for DB context

        //Scalar Properties
        [Display(Name = "Unique Number:")]
        [Range(0, int.MaxValue, ErrorMessage = "Book number cannot be negative.")]
        public int BookNumber { get; set; }

        [Display(Name = "Title:")]
        public string Title { get; set; } // Title of the book

        
        [Display(Name = "Description:")]
        public string? BookDescription { get; set; }


        //EXAMPLES: Book_Price, BookClass_Price, etc.
        [Display(Name = "Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be below 0.")]
        public decimal BookPrice { get; set; }

        [Display(Name = "Cost:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be below 0.")]
        public decimal BookCost { get; set; }

        [Display(Name = "Publish Date:")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "Inventory Quantity:")]
        [Range(0, int.MaxValue, ErrorMessage = "Inventory quantity cannot be negative.")]
        public int InventoryQuantity { get; set; }

        [Display(Name = "Reorder Point:")]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder point cannot be negative.")]
        public int ReorderPoint { get; set; }


        //Is authors a list of strings? or just a string? ( SHOULD BE STRING RIGHT? becuase list would mean its a list of objects unless list of strings)
        // Authors is just a string.
        [Display(Name = "Authors:")]
        public string Authors { get; set; }
        //public List<string> AuthorList { get; set; }

        [Display(Name = "Status (Active):")]
        public bool BookStatus { get; set; } //Book status is bool representing whether the book is discontinued or not
                                             // True = Still Active , False = Discontinued,inactive

        //NAVIGATIONAL PROPERTIES
        //Genre
        public Genres? genre { get; set; } //Genre books side is the many side, therefore genre is single 

        //Reviews
        public List<Reviews>? reviews { get; set; } = new List<Reviews>();
        //Reviews-Book relationship 1 to many (optional) ; is that a book can have many reviews and a review is tied to 1 book

        //OrderDetails MANY TO MANY
        public List<OrderDetails>? orderDetails { get; set; }
        //This uses OrderDetails to create linking table to Orders class (many to many relationship)


        [NotMapped]
        [Display(Name = "Rating")]
        public decimal AverageRating
        {

            get
            {
                if (reviews == null || reviews.Count() == 0)
                {
                    return 0m;  // ← Return null instead of 0m
                }

                decimal sum = 0m;

                foreach (var r in reviews)
                {
                    sum += (decimal)r.Rating;
                }

                return Math.Round(sum / reviews.Count, 1);

            }
            

        }


    }
}


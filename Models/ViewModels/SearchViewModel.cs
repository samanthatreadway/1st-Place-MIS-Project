using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
	public enum PriceComparisonEnum
	{
		[Display(Name = "Higher")]
		greaterThan,
		[Display(Name = "Lower")]
		lessThan
	}

	public enum RatingComparisonEnum
	{
        [Display(Name = "Higher")]
        greaterThan,
        [Display(Name = "Lower")]
        lessThan
    }

	public class SearchViewModel
	{
		
		public string? Title { get; set; }

		[Display(Name = "Description:")]
		public string? Desc { get; set; }

        [Range(0.01, 9999999999.99, ErrorMessage = "Please enter a valid price")]
		public decimal? Price { get; set; }

        [Display(Name = "")]
        public PriceComparisonEnum? priceComparison { get; set; }

		public DateTime? Date { get; set; }

		public string? Author { get; set; }


		//holds Genre ID
		public int? Genre { get; set; }

		[Display(Name = "Book Unique Number: ")]
		public int? Number { get; set; }

        [Display(Name = "In Stock: ")]
        public bool Stock { get; set; }

		
		public RatingComparisonEnum? Rating { get; set; }

		[Display( Name = "Book Rating")]
		[Range(typeof(decimal), "0", "5", ErrorMessage = "Please Enter Rating 0-5")]
		public decimal? userRating { get; set; }
	
	}
}


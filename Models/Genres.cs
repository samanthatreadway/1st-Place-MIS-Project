using System;
using System.ComponentModel.DataAnnotations;


namespace fa25group23final.Models
{
	public class Genres
	{

		[Key]
		public int GenreID { get; set; } //Primary Key

        [Display(Name = "Genre:")]
		public string GenreName { get; set; } //Name of Genre

        //Navigational Properties
        public List<Books> books { get; set; } = new List<Books>();

        public static implicit operator Genres(int v)
        {
            throw new NotImplementedException();
        }
        //Books genre is 1 to many ; with a genre having many books but a Books having 1 genre

    }
}


using System;
using System.ComponentModel.DataAnnotations;
namespace fa25group23final.Models
{
	public class Reorders
	{
            [Key]
            public int ReorderID { get; set; }

            [Display(Name = "Order Date")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime? OrderDate { get; set; } // null = draft/not yet placed

        // Navigational Properties
        public ICollection<ReorderDetails> ReorderDetails { get; set; } = new List<ReorderDetails>();
    }
}


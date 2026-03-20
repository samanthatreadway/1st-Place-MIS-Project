using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
    public class ReorderDetails
    {
        [Key]
        public int ReorderDetailsID { get; set; }

        // Scalar Properties
        [Display(Name = "Quantity Ordered")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be zero or greater")]
        public int QuantityOrdered { get; set; } = 5; // default per spec

        [Display(Name = "Quantity Received")]
        public int QuantityReceived { get; set; } = 0;

        [Display(Name = "Cost")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero")]
        public decimal Cost { get; set; }

        // Navigational Properties
        public Reorders Reorder { get; set; }
        public Books Book { get; set; }
    }
}

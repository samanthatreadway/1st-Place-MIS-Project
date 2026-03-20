using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
    public class Shippingconfig
    {
        [Key]
        public int ShippingConfigID { get; set; }

        [Display(Name = "First Book Shipping Fee:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        [Range(0, 100, ErrorMessage = "Shipping fee must be between $0 and $100")]
        public decimal FirstBookFee { get; set; } = 3.50m;

        [Display(Name = "Additional Book Shipping Fee:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        [Range(0, 100, ErrorMessage = "Shipping fee must be between $0 and $100")]
        public decimal AdditionalBookFee { get; set; } = 1.50m;

        [Display(Name = "Updated By:")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Is Active:")]
        public bool IsActive { get; set; } = true;
    }
}

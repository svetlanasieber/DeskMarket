using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class ProductFormViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1.00, 3000.00)]
        public decimal Price { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Added On")]
        public DateTime AddedOn { get; set; }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
} 
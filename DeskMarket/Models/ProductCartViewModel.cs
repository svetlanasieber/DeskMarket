using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class ProductCartViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;
    }
} 
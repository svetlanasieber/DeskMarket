using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class ProductDetailsViewModel
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

        [Display(Name = "Seller")]
        public string SellerName { get; set; } = null!;

        public string SellerId { get; set; } = null!;

        [Display(Name = "Added On")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime AddedOn { get; set; }

        public bool IsInCart { get; set; }
    }
} 
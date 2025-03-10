using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;
    }
} 
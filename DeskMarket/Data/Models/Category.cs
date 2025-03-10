using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Data.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
} 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArteEva.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation Properties
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}

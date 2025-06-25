using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TypicalTechTools.Models
{
    public class Product
    {
        [Key]
        [Required]
        [Column("product_code")]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        // Ensures the price is within a valid range
        [Range(0, 9999.99, ErrorMessage = "The price must be between 0 and 9999.99.")]  
        [Column("product_price")]
        public decimal ProductPrice { get; set; }

        [Column("product_description")]
        public string? ProductDescription { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
    }
}

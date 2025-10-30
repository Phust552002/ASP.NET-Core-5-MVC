using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ASP.NETCore5.Validators;

namespace ASP.NETCore5.Models
{
    public class Product
    {
        [Display(Name = "Product Id")]
        [Required]
        public int Id { set; get; }

        [Display(Name = "Product Name")]
        [Required]
        public string Name { set; get; }

        [Display(Name = "Price")]
        [Required]
        public float Price { set; get; }

        [Display(Name = "Quantity")]
        [Required]
        [StockValidation]
        public int Quantity { set; get; }

    }
}

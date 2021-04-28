using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Application.Carts
{
    public class ProductDto
    {
        [Required(ErrorMessage = "The {0} field is mandatory")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The {0} field is mandatory")]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Price value is invalid")]
        public int Quantity { get; set; }

        public ProductDto(Guid id, int quantity)
        {
            Id = id;
            Quantity = quantity;
        }
    }
}
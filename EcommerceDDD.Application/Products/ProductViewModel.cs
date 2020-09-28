using System;

namespace EcommerceDDD.Application.Customers.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string CurrencySymbol { get; set; }
    }
}

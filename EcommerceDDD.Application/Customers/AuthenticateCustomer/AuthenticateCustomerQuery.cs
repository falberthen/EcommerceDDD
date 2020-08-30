using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Application.Base.Queries;
using EcommerceDDD.Application.Customers.ViewModels;

namespace EcommerceDDD.Application.Customers.AuthenticateCustomer
{
    public class AuthenticateCustomerQuery : IQuery<CustomerViewModel>
    {
        public AuthenticateCustomerQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }
}

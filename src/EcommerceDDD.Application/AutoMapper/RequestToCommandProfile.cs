using AutoMapper;
using EcommerceDDD.Application.Customers.RegisterCustomer;
using EcommerceDDD.Application.Customers.UpdateCustomer;
using EcommerceDDD.Application.Orders.PlaceOrder;

namespace EcommerceDDD.Application.AutoMapper
{
    public class RequestToCommandProfile : Profile
    {
        public RequestToCommandProfile()
        {
            CreateMap<RegisterCustomerRequest, RegisterCustomerCommand>()
            .ConstructUsing(c => new RegisterCustomerCommand(c.Email, c.Name, c.Password, c.PasswordConfirm));
        }
    }
}

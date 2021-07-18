using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using EcommerceDDD.Infrastructure.Identity.Services;
using EcommerceDDD.Application.Customers.ViewModels;
using EcommerceDDD.Infrastructure.Identity.IdentityUser;
using EcommerceDDD.Application.Base;
using BuildingBlocks.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Customers.AuthenticateCustomer
{
    public class AuthenticateCustomerQueryHandler : QueryHandler<AuthenticateCustomerQuery, CustomerViewModel> 
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ICustomers _customers;
        private readonly IJwtService _jwtService;

        public AuthenticateCustomerQueryHandler(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ICustomers customers,
            IJwtService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _customers = customers;
            _jwtService = jwtService;
        }

        public async override Task<CustomerViewModel> ExecuteQuery(AuthenticateCustomerQuery request, CancellationToken cancellationToken)
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();

            var signIn = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
            if (signIn.Succeeded)            
            {
                var token = await _jwtService.GenerateJwt(request.Email);
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    throw new InvalidDataException("User not found.");

                //Customer data
                var customer = await _customers
                    .GetByEmail(user.Email, cancellationToken);

                customerViewModel.Id = customer.Id.Value;
                customerViewModel.Name = customer.Name;
                customerViewModel.Email = user.Email;
                customerViewModel.Token = token;
                customerViewModel.LoginSucceeded = signIn.Succeeded;
            }
            else
                customerViewModel.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, 
                    "Usename or password invalid"));

            return customerViewModel;
        }
    }
}

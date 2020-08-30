using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base.Queries;
using EcommerceDDD.Domain.Customers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using EcommerceDDD.Infrastructure.Identity.Services;
using EcommerceDDD.Application.Customers.ViewModels;
using EcommerceDDD.Infrastructure.Identity.IdentityUser;

namespace EcommerceDDD.Application.Customers.AuthenticateCustomer
{
    public class AuthenticateCustomerQueryHandler : IQueryHandler<AuthenticateCustomerQuery, CustomerViewModel> 
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IJwtService _jwtService;

        public AuthenticateCustomerQueryHandler(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ICustomerRepository customerRepository,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
        }

        public async Task<CustomerViewModel> Handle(AuthenticateCustomerQuery request, CancellationToken cancellationToken)
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();

            var signIn = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
            if (signIn.Succeeded)            
            {
                var token = await _jwtService.GenerateJwt(request.Email);
                var user = await _userManager.FindByEmailAsync(request.Email);

                //Customer data
                var customer = await _customerRepository.GetCustomerByEmail(user.Email);
                customerViewModel.Id = customer.Id;
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

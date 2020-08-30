using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base.Commands;
using EcommerceDDD.Domain.Customers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EcommerceDDD.Domain;
using EcommerceDDD.Infrastructure.Identity.IdentityUser;
using Microsoft.AspNetCore.Identity;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : CommandHandler<RegisterCustomerCommand, CommandHandlerResult>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICustomerUniquenessChecker _uniquenessChecker;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterCustomerCommandHandler(
            UserManager<User> userManager,
            IEcommerceUnitOfWork unitOfWork,
            ICustomerUniquenessChecker uniquenessChecker,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _uniquenessChecker = uniquenessChecker;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public override async Task<Guid> RunCommand(RegisterCustomerCommand command, CancellationToken cancellationToken)
        {
            Customer customer = Customer.CreateCustomer(command.Email, command.Name, _uniquenessChecker);            
            if(customer != null)
            {
                await _unitOfWork.CustomerRepository.RegisterCustomer(customer, cancellationToken);
                if (await _unitOfWork.CommitAsync())
                    await CreateUserForCustomer(command);
            }

            return customer.Id;
        }

        private async Task<User> CreateUserForCustomer(RegisterCustomerCommand request)
        {
            //Creating Identity user
            var user = new User(_httpContextAccessor)
            {
                UserName = request.Email,
                Email = request.Email
            };

            var userCreated = await _userManager.CreateAsync(user, request.Password);
            if (userCreated.Succeeded)
            {
                //Adding user claims
                await _userManager.AddClaimAsync(user, new Claim("CanRead", "Read"));
                await _userManager.AddClaimAsync(user, new Claim("CanSave", "Save"));
                await _userManager.AddClaimAsync(user, new Claim("CanDelete", "Delete"));
            }

            return user;
        }
    }
}

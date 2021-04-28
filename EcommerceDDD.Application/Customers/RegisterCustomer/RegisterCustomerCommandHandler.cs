using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EcommerceDDD.Domain;
using EcommerceDDD.Infrastructure.Identity.IdentityUser;
using Microsoft.AspNetCore.Identity;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Application.Base;
using BuildingBlocks.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : CommandHandler<RegisterCustomerCommand, Guid>
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
        }

        public override async Task<Guid> ExecuteCommand(RegisterCustomerCommand command, CancellationToken cancellationToken)
        {
            var customer = Customer.CreateCustomer(Guid.NewGuid(), command.Email, command.Name, _uniquenessChecker);            
            if(customer != null)
            {
                await _unitOfWork.CustomerRepository.Add(customer, cancellationToken);
                await CreateUserForCustomer(command);
                await _unitOfWork.CommitAsync();
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
            if (!userCreated.Succeeded)
            {
                foreach (var error in userCreated.Errors)
                {
                    throw new InvalidDataException(error.Description.ToString());
                }
            }
 
            //Adding user claims
            await _userManager.AddClaimAsync(user, new Claim("CanRead", "Read"));
            await _userManager.AddClaimAsync(user, new Claim("CanSave", "Save"));
            await _userManager.AddClaimAsync(user, new Claim("CanDelete", "Delete"));            

            return user;
        }
    }
}

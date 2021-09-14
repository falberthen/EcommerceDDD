using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EcommerceDDD.Domain;
using Microsoft.AspNetCore.Identity;
using EcommerceDDD.Application.Base;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Infrastructure.Identity.Users;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : CommandHandler<RegisterCustomerCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICustomerUniquenessChecker _uniquenessChecker;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterCustomerCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEcommerceUnitOfWork unitOfWork,
            ICustomerUniquenessChecker uniquenessChecker,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _uniquenessChecker = uniquenessChecker;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<Guid> ExecuteCommand(RegisterCustomerCommand command, 
            CancellationToken cancellationToken)
        {
            try
            {
                var customer = Customer.CreateNew(
                    command.Email, 
                    command.Name, 
                    _uniquenessChecker
                );

                if (customer != null)
                {
                    await _unitOfWork.Customers
                        .Add(customer, cancellationToken);

                    await CreateUserForCustomer(command);
                    await _unitOfWork.CommitAsync();
                }

                return customer.Id.Value;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private async Task<ApplicationUser> CreateUserForCustomer(RegisterCustomerCommand request)
        {
            //Creating Identity user
            var user = new ApplicationUser(_httpContextAccessor)
            {
                UserName = request.Email,
                Email = request.Email
            };

            var userCreated = await _userManager
                .CreateAsync(user, request.Password);

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

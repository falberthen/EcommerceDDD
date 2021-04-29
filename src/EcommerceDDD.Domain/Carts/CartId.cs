using EcommerceDDD.Domain.SeedWork;
using System;

namespace EcommerceDDD.Domain.Carts
{
    public class CartId : StronglyTypedId<CartId>
    {
        public CartId(Guid value) : base(value)
        {
        }

        public static CartId Of(Guid cartId)
        {
            if (cartId == Guid.Empty)
                throw new BusinessRuleException("Cart Id must be provided.");

            return new CartId(cartId);
        }
    }
}

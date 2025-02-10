using Stripe.Checkout;
using PetProjectOne.Services;
using PetProjectOne.Entities;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Session> CreateStripeCheckoutSession(User user, decimal amount)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{_configuration["ClientUrl"]}/payment/success.html",
            CancelUrl = $"{_configuration["ClientUrl"]}/payment/failed.html",
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100), // Convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = user.FullName ?? "Tasker",
                            Description = "Tasker Service Payment"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment"
        };

        var service = new SessionService();
        return await service.CreateAsync(options);
    }

}

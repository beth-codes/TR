using System.Security.Claims;
using PetProjectOne.Models;
using PetProjectOne.Entities;
using Stripe.Checkout;

namespace PetProjectOne.Services;

public interface IPaymentService
{
    Task<Session> CreateStripeCheckoutSession(User user, decimal amount);
    
}

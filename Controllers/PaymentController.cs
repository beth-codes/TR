using Microsoft.AspNetCore.Mvc;
using PetProjectOne.Services; 
using PetProjectOne.Models; 
namespace PetProjectOne.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userServices;

    public PaymentController(IPaymentService paymentService, IConfiguration configuration, IUserService userService)
    {
        _paymentService = paymentService;
        _configuration = configuration;
        _userServices = userService;
    }

    [HttpPost("create-checkout-session")]
    public async Task<ActionResult> CreateCheckoutSession([FromBody] PaymentRequest request)
    {
        // Fetch tasker details from your service
        var tasker = (await _userServices.GetAllTaskers())
            .FirstOrDefault(t => t.Id == request.TaskerId);

        if (tasker == null)
        {
            return NotFound("Tasker not found.");
        }

        // Use the associated User from the TaskerProfile
        var user = tasker.User;

        if (user == null)
        {
            return StatusCode(500, "Tasker has no associated user.");
        }

        // Pass the User object to CreateStripeCheckoutSession
        var session = await _paymentService.CreateStripeCheckoutSession(user, request.Amount);
        if (session == null)
        {
            return StatusCode(500, "Failed to create checkout session.");
        }

        return Ok(new
        {
            SessionId = session.Id,
            PubKey = _configuration["Stripe:PubKey"]
        });
    }

}

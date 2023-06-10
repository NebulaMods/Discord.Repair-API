using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Mvc;

using Stripe;

namespace DiscordRepair.Api.Endpoints.V1;

[Route("/v1/")]
[ApiController]
[ApiExplorerSettings(GroupName = "API Endpoints")]
public class PaymentGateway : ControllerBase
{
    [HttpPost("payment-gateway")]
    [Consumes("plain/text")]
    [Produces("plain/text")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<IActionResult> HandleAsync()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            #if DEBUG
            var stripe = new StripeClient("sk_test_5EdGdpHfeOaPAg2UX1vfvXDc");
            #else
            var stripe = new StripeClient("sk_live_pB89OdCFVeuKDQ3H2qn4TShQ");
            #endif
            var stripeEvent = EventUtility.ParseEvent(json);
            //whsec_Yb7OlG9InxeQpLlDwf1sO27h74bt9cKo
            //whsec_c8450c1a78300991810289d7bd219753e6dd627d9fd12105cc0974ce8831e31d
            // Handle the event
            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Then define and call a method to handle the successful payment intent.
                // handlePaymentIntentSucceeded(paymentIntent);
            }
            else if (stripeEvent.Type == Events.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                // Then define and call a method to handle the successful attachment of a PaymentMethod.
                // handlePaymentMethodAttached(paymentMethod);
            }
            // ... handle other event types
            else
            {
                // Unexpected event type
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }
}

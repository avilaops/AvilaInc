using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Infrastructure.Services;
using Stripe;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/stripe")]
[Authorize]
public class StripeController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly ILogger<StripeController> _logger;
    private readonly IConfiguration _configuration;

    public StripeController(
        IStripeService stripeService,
        ILogger<StripeController> logger,
        IConfiguration configuration)
    {
        _stripeService = stripeService;
        _logger = logger;
        _configuration = configuration;
    }

    #region Payment Intents

    /// <summary>
    /// Listar Payment Intents
    /// </summary>
    [HttpGet("payments")]
    public async Task<IActionResult> ListPaymentIntents(
        [FromQuery] int limit = 10,
        [FromQuery] string? customerId = null)
    {
        try
        {
            var paymentIntents = await _stripeService.ListPaymentIntentsAsync(limit, customerId);

            return Ok(new
            {
                success = true,
                count = paymentIntents.Data.Count,
                hasMore = paymentIntents.HasMore,
                data = paymentIntents.Data.Select(pi => new
                {
                    id = pi.Id,
                    amount = pi.Amount,
                    currency = pi.Currency,
                    status = pi.Status,
                    customerId = pi.CustomerId,
                    created = pi.Created,
                    metadata = pi.Metadata
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar PaymentIntents");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar Payment Intent
    /// </summary>
    [HttpPost("payments")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        try
        {
            var paymentIntent = await _stripeService.CreatePaymentIntentAsync(
                request.Amount,
                request.Currency ?? "brl",
                request.CustomerId,
                request.Metadata
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = paymentIntent.Id,
                    clientSecret = paymentIntent.ClientSecret,
                    amount = paymentIntent.Amount,
                    currency = paymentIntent.Currency,
                    status = paymentIntent.Status
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar PaymentIntent");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Buscar Payment Intent por ID
    /// </summary>
    [HttpGet("payments/{id}")]
    public async Task<IActionResult> GetPaymentIntent(string id)
    {
        try
        {
            var paymentIntent = await _stripeService.GetPaymentIntentAsync(id);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = paymentIntent.Id,
                    amount = paymentIntent.Amount,
                    currency = paymentIntent.Currency,
                    status = paymentIntent.Status,
                    customerId = paymentIntent.CustomerId,
                    paymentMethodId = paymentIntent.PaymentMethodId,
                    created = paymentIntent.Created,
                    metadata = paymentIntent.Metadata
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar PaymentIntent {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Confirmar Payment Intent
    /// </summary>
    [HttpPost("payments/{id}/confirm")]
    public async Task<IActionResult> ConfirmPaymentIntent(
        string id,
        [FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var paymentIntent = await _stripeService.ConfirmPaymentIntentAsync(id, request.PaymentMethodId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = paymentIntent.Id,
                    status = paymentIntent.Status,
                    amount = paymentIntent.Amount,
                    currency = paymentIntent.Currency
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar PaymentIntent {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar Payment Intent
    /// </summary>
    [HttpPost("payments/{id}/cancel")]
    public async Task<IActionResult> CancelPaymentIntent(string id)
    {
        try
        {
            var paymentIntent = await _stripeService.CancelPaymentIntentAsync(id);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = paymentIntent.Id,
                    status = paymentIntent.Status
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar PaymentIntent {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Customers

    /// <summary>
    /// Listar Customers
    /// </summary>
    [HttpGet("customers")]
    public async Task<IActionResult> ListCustomers(
        [FromQuery] int limit = 10,
        [FromQuery] string? email = null)
    {
        try
        {
            var customers = await _stripeService.ListCustomersAsync(limit, email);

            return Ok(new
            {
                success = true,
                count = customers.Data.Count,
                hasMore = customers.HasMore,
                data = customers.Data.Select(c => new
                {
                    id = c.Id,
                    email = c.Email,
                    name = c.Name,
                    created = c.Created,
                    metadata = c.Metadata
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar Customers");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar Customer
    /// </summary>
    [HttpPost("customers")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = await _stripeService.CreateCustomerAsync(
                request.Email,
                request.Name,
                request.Metadata
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = customer.Id,
                    email = customer.Email,
                    name = customer.Name,
                    created = customer.Created
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar Customer");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Buscar Customer por ID
    /// </summary>
    [HttpGet("customers/{id}")]
    public async Task<IActionResult> GetCustomer(string id)
    {
        try
        {
            var customer = await _stripeService.GetCustomerAsync(id);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = customer.Id,
                    email = customer.Email,
                    name = customer.Name,
                    phone = customer.Phone,
                    created = customer.Created,
                    metadata = customer.Metadata
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar Customer {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar Customer
    /// </summary>
    [HttpPut("customers/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _stripeService.UpdateCustomerAsync(id, request.Email, request.Name);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = customer.Id,
                    email = customer.Email,
                    name = customer.Name
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar Customer {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Listar Payment Methods de um Customer
    /// </summary>
    [HttpGet("customers/{id}/payment-methods")]
    public async Task<IActionResult> ListPaymentMethods(string id)
    {
        try
        {
            var paymentMethods = await _stripeService.ListPaymentMethodsAsync(id);

            return Ok(new
            {
                success = true,
                count = paymentMethods.Data.Count,
                data = paymentMethods.Data.Select(pm => new
                {
                    id = pm.Id,
                    type = pm.Type,
                    card = pm.Card != null ? new
                    {
                        brand = pm.Card.Brand,
                        last4 = pm.Card.Last4,
                        expMonth = pm.Card.ExpMonth,
                        expYear = pm.Card.ExpYear
                    } : null
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar PaymentMethods do Customer {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Anexar Payment Method a um Customer
    /// </summary>
    [HttpPost("payment-methods/{paymentMethodId}/attach")]
    public async Task<IActionResult> AttachPaymentMethod(
        string paymentMethodId,
        [FromBody] AttachPaymentMethodRequest request)
    {
        try
        {
            var paymentMethod = await _stripeService.AttachPaymentMethodAsync(
                paymentMethodId,
                request.CustomerId
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = paymentMethod.Id,
                    customerId = paymentMethod.CustomerId
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao anexar PaymentMethod {PaymentMethodId}", paymentMethodId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Balance & Charges

    /// <summary>
    /// Obter Balance da conta Stripe
    /// </summary>
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        try
        {
            var balance = await _stripeService.GetBalanceAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    available = balance.Available.Select(b => new
                    {
                        amount = b.Amount,
                        currency = b.Currency
                    }),
                    pending = balance.Pending.Select(b => new
                    {
                        amount = b.Amount,
                        currency = b.Currency
                    })
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter Balance");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Listar Charges
    /// </summary>
    [HttpGet("charges")]
    public async Task<IActionResult> ListCharges(
        [FromQuery] int limit = 10,
        [FromQuery] string? customerId = null)
    {
        try
        {
            var charges = await _stripeService.ListChargesAsync(limit, customerId);

            return Ok(new
            {
                success = true,
                count = charges.Data.Count,
                hasMore = charges.HasMore,
                data = charges.Data.Select(c => new
                {
                    id = c.Id,
                    amount = c.Amount,
                    currency = c.Currency,
                    status = c.Status,
                    customerId = c.CustomerId,
                    paymentMethod = c.PaymentMethod,
                    created = c.Created
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar Charges");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Webhook

    /// <summary>
    /// Webhook do Stripe (AllowAnonymous)
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
            var webhookSecret = _configuration["Integrations:Stripe:WebhookSecret"] ?? "";

            var stripeEvent = _stripeService.ConstructWebhookEvent(json, stripeSignature, webhookSecret);

            _logger.LogInformation("Stripe Webhook recebido: {EventType}", stripeEvent.Type);

            // Processar eventos importantes
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation("PaymentIntent succeeded: {Id}", paymentIntent?.Id);
                    // TODO: Atualizar ClientFinance no MongoDB
                    break;

                case "payment_intent.payment_failed":
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogWarning("PaymentIntent failed: {Id}", failedIntent?.Id);
                    // TODO: Notificar falha no pagamento
                    break;

                case "customer.created":
                    var customer = stripeEvent.Data.Object as Customer;
                    _logger.LogInformation("Customer created: {Id}", customer?.Id);
                    break;

                case "charge.succeeded":
                    var charge = stripeEvent.Data.Object as Charge;
                    _logger.LogInformation("Charge succeeded: {Id} - {Amount}", charge?.Id, charge?.Amount);
                    break;

                default:
                    _logger.LogInformation("Evento não processado: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok(new { received = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar Webhook do Stripe");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public record CreatePaymentIntentRequest(
    long Amount,
    string CustomerId,
    string? Currency = "brl",
    Dictionary<string, string>? Metadata = null
);

public record ConfirmPaymentRequest(string PaymentMethodId);

public record CreateCustomerRequest(
    string Email,
    string Name,
    Dictionary<string, string>? Metadata = null
);

public record UpdateCustomerRequest(string? Email = null, string? Name = null);

public record AttachPaymentMethodRequest(string CustomerId);

#endregion

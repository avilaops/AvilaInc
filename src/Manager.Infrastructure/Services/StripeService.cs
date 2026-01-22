using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Manager.Infrastructure.Services;

public interface IStripeService
{
    // Payment Intents
    Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency, string customerId, Dictionary<string, string>? metadata = null);
    Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
    Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId);
    Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId);
    Task<StripeList<PaymentIntent>> ListPaymentIntentsAsync(int limit = 10, string? customerId = null);

    // Customers
    Task<Customer> CreateCustomerAsync(string email, string name, Dictionary<string, string>? metadata = null);
    Task<Customer> GetCustomerAsync(string customerId);
    Task<Customer> UpdateCustomerAsync(string customerId, string? email = null, string? name = null);
    Task<StripeList<Customer>> ListCustomersAsync(int limit = 10, string? email = null);

    // Payment Methods
    Task<PaymentMethod> AttachPaymentMethodAsync(string paymentMethodId, string customerId);
    Task<StripeList<PaymentMethod>> ListPaymentMethodsAsync(string customerId);

    // Balance
    Task<Balance> GetBalanceAsync();

    // Charges
    Task<StripeList<Charge>> ListChargesAsync(int limit = 10, string? customerId = null);

    // Webhook
    Event ConstructWebhookEvent(string json, string stripeSignature, string webhookSecret);
}

public class StripeService : IStripeService
{
    private readonly ILogger<StripeService> _logger;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly CustomerService _customerService;
    private readonly PaymentMethodService _paymentMethodService;
    private readonly BalanceService _balanceService;
    private readonly ChargeService _chargeService;

    public StripeService(IConfiguration configuration, ILogger<StripeService> logger)
    {
        _logger = logger;

        var secretKey = configuration["Integrations:Stripe:SecretKey"] 
                       ?? throw new ArgumentException("Stripe SecretKey não configurada");

        StripeConfiguration.ApiKey = secretKey;

        _paymentIntentService = new PaymentIntentService();
        _customerService = new CustomerService();
        _paymentMethodService = new PaymentMethodService();
        _balanceService = new BalanceService();
        _chargeService = new ChargeService();

        _logger.LogInformation("StripeService inicializado com sucesso");
    }

    #region Payment Intents

    public async Task<PaymentIntent> CreatePaymentIntentAsync(
        long amount,
        string currency,
        string customerId,
        Dictionary<string, string>? metadata = null)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Customer = customerId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = metadata
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options);
            _logger.LogInformation("PaymentIntent criado: {PaymentIntentId} - {Amount} {Currency}",
                paymentIntent.Id, amount, currency);

            return paymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao criar PaymentIntent");
            throw;
        }
    }

    public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            return await _paymentIntentService.GetAsync(paymentIntentId);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao buscar PaymentIntent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId)
    {
        try
        {
            var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodId
            };

            var paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntentId, options);
            _logger.LogInformation("PaymentIntent confirmado: {PaymentIntentId}", paymentIntentId);

            return paymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao confirmar PaymentIntent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    public async Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var paymentIntent = await _paymentIntentService.CancelAsync(paymentIntentId);
            _logger.LogInformation("PaymentIntent cancelado: {PaymentIntentId}", paymentIntentId);

            return paymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao cancelar PaymentIntent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    public async Task<StripeList<PaymentIntent>> ListPaymentIntentsAsync(int limit = 10, string? customerId = null)
    {
        try
        {
            var options = new PaymentIntentListOptions
            {
                Limit = limit
            };

            if (!string.IsNullOrEmpty(customerId))
            {
                options.Customer = customerId;
            }

            return await _paymentIntentService.ListAsync(options);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao listar PaymentIntents");
            throw;
        }
    }

    #endregion

    #region Customers

    public async Task<Customer> CreateCustomerAsync(
        string email,
        string name,
        Dictionary<string, string>? metadata = null)
    {
        try
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = metadata
            };

            var customer = await _customerService.CreateAsync(options);
            _logger.LogInformation("Customer criado: {CustomerId} - {Email}", customer.Id, email);

            return customer;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao criar Customer");
            throw;
        }
    }

    public async Task<Customer> GetCustomerAsync(string customerId)
    {
        try
        {
            return await _customerService.GetAsync(customerId);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao buscar Customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<Customer> UpdateCustomerAsync(string customerId, string? email = null, string? name = null)
    {
        try
        {
            var options = new CustomerUpdateOptions();

            if (!string.IsNullOrEmpty(email))
                options.Email = email;

            if (!string.IsNullOrEmpty(name))
                options.Name = name;

            var customer = await _customerService.UpdateAsync(customerId, options);
            _logger.LogInformation("Customer atualizado: {CustomerId}", customerId);

            return customer;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao atualizar Customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<StripeList<Customer>> ListCustomersAsync(int limit = 10, string? email = null)
    {
        try
        {
            var options = new CustomerListOptions
            {
                Limit = limit
            };

            if (!string.IsNullOrEmpty(email))
            {
                options.Email = email;
            }

            return await _customerService.ListAsync(options);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao listar Customers");
            throw;
        }
    }

    #endregion

    #region Payment Methods

    public async Task<PaymentMethod> AttachPaymentMethodAsync(string paymentMethodId, string customerId)
    {
        try
        {
            var options = new PaymentMethodAttachOptions
            {
                Customer = customerId
            };

            var paymentMethod = await _paymentMethodService.AttachAsync(paymentMethodId, options);
            _logger.LogInformation("PaymentMethod {PaymentMethodId} anexado ao Customer {CustomerId}",
                paymentMethodId, customerId);

            return paymentMethod;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao anexar PaymentMethod");
            throw;
        }
    }

    public async Task<StripeList<PaymentMethod>> ListPaymentMethodsAsync(string customerId)
    {
        try
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card"
            };

            return await _paymentMethodService.ListAsync(options);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao listar PaymentMethods do Customer {CustomerId}", customerId);
            throw;
        }
    }

    #endregion

    #region Balance

    public async Task<Balance> GetBalanceAsync()
    {
        try
        {
            var balance = await _balanceService.GetAsync();
            _logger.LogInformation("Balance obtido: {Available} disponível",
                balance.Available.Sum(b => b.Amount));

            return balance;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao obter Balance");
            throw;
        }
    }

    #endregion

    #region Charges

    public async Task<StripeList<Charge>> ListChargesAsync(int limit = 10, string? customerId = null)
    {
        try
        {
            var options = new ChargeListOptions
            {
                Limit = limit
            };

            if (!string.IsNullOrEmpty(customerId))
            {
                options.Customer = customerId;
            }

            return await _chargeService.ListAsync(options);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao listar Charges");
            throw;
        }
    }

    #endregion

    #region Webhooks

    public Event ConstructWebhookEvent(string json, string stripeSignature, string webhookSecret)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            _logger.LogInformation("Webhook event recebido: {EventType}", stripeEvent.Type);

            return stripeEvent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao processar Webhook");
            throw;
        }
    }

    #endregion
}

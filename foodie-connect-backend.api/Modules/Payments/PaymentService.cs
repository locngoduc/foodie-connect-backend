using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Patterns.Adapter;
using foodie_connect_backend.Shared.Patterns.Decorator;

namespace foodie_connect_backend.Modules.Payments;

/// <summary>
/// Payment service that demonstrates the Adapter pattern
/// Uses different payment gateway adapters through a unified interface
/// </summary>
public class PaymentService : BaseLoggableService
{
    private readonly Dictionary<string, IPaymentGateway> _paymentGateways;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        StripePaymentAdapter stripeAdapter,
        PayPalPaymentAdapter payPalAdapter,
        SquarePaymentAdapter squareAdapter,
        ILogger<PaymentService> logger)
    {
        _logger = logger;
        _paymentGateways = new Dictionary<string, IPaymentGateway>(StringComparer.OrdinalIgnoreCase)
        {
            { "stripe", stripeAdapter },
            { "paypal", payPalAdapter },
            { "square", squareAdapter }
        };
    }

    /// <summary>
    /// Process payment using the specified gateway
    /// Demonstrates how the Adapter pattern allows using different payment gateways
    /// through a unified interface
    /// </summary>
    public async Task<Result<PaymentResult>> ProcessPaymentAsync(
        string gatewayName,
        decimal amount,
        string currency,
        string paymentMethodId,
        string description)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            if (!_paymentGateways.TryGetValue(gatewayName, out var gateway))
            {
                return Result<PaymentResult>.Failure(
                    new AppError("UNSUPPORTED_GATEWAY", $"Payment gateway '{gatewayName}' is not supported"));
            }

            _logger.LogInformation("Processing payment through {Gateway} for amount {Amount} {Currency}", 
                gateway.GatewayName, amount, currency);

            return await gateway.ProcessPaymentAsync(amount, currency, paymentMethodId, description);
        }, $"ProcessPayment-{gatewayName}");
    }

    /// <summary>
    /// Process refund using the specified gateway
    /// </summary>
    public async Task<Result<RefundResult>> RefundPaymentAsync(
        string gatewayName,
        string transactionId,
        decimal? amount = null,
        string? reason = null)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            if (!_paymentGateways.TryGetValue(gatewayName, out var gateway))
            {
                return Result<RefundResult>.Failure(
                    new AppError("UNSUPPORTED_GATEWAY", $"Payment gateway '{gatewayName}' is not supported"));
            }

            _logger.LogInformation("Processing refund through {Gateway} for transaction {TransactionId}", 
                gateway.GatewayName, transactionId);

            return await gateway.RefundPaymentAsync(transactionId, amount, reason);
        }, $"RefundPayment-{gatewayName}");
    }

    /// <summary>
    /// Get payment status using the specified gateway
    /// </summary>
    public async Task<Result<PaymentStatus>> GetPaymentStatusAsync(
        string gatewayName,
        string transactionId)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            if (!_paymentGateways.TryGetValue(gatewayName, out var gateway))
            {
                return Result<PaymentStatus>.Failure(
                    new AppError("UNSUPPORTED_GATEWAY", $"Payment gateway '{gatewayName}' is not supported"));
            }

            return await gateway.GetPaymentStatusAsync(transactionId);
        }, $"GetPaymentStatus-{gatewayName}");
    }

    /// <summary>
    /// Get all available payment gateways and their capabilities
    /// </summary>
    public async Task<Result<List<PaymentGatewayInfo>>> GetAvailableGatewaysAsync()
    {
        return await ExecuteWithResultAsync(async () =>
        {
            await Task.Delay(1); // Simulate async operation
            
            var gateways = _paymentGateways.Values
                .Select(gateway => new PaymentGatewayInfo
                {
                    Name = gateway.GatewayName,
                    SupportedCurrencies = gateway.SupportedCurrencies
                })
                .ToList();

            return Result<List<PaymentGatewayInfo>>.Success(gateways);
        }, "GetAvailableGateways");
    }

    /// <summary>
    /// Find the best gateway for a specific currency
    /// Demonstrates how the Adapter pattern allows comparing different implementations
    /// </summary>
    public async Task<Result<string>> FindBestGatewayForCurrencyAsync(string currency)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            await Task.Delay(1); // Simulate async operation
            
            var supportingGateways = _paymentGateways.Values
                .Where(gateway => gateway.SupportedCurrencies.Contains(currency.ToUpper()))
                .ToList();

            if (!supportingGateways.Any())
            {
                return Result<string>.Failure(
                    new AppError("NO_GATEWAY_SUPPORTS_CURRENCY", $"No gateway supports currency {currency}"));
            }

            // Simple logic: prefer Stripe, then PayPal, then Square
            var preferredOrder = new[] { "Stripe", "PayPal", "Square" };
            var bestGateway = supportingGateways
                .OrderBy(g => Array.IndexOf(preferredOrder, g.GatewayName))
                .First();

            return Result<string>.Success(bestGateway.GatewayName);
        }, "FindBestGatewayForCurrency");
    }

    /// <summary>
    /// Process payment with automatic gateway selection
    /// Demonstrates the power of the Adapter pattern in providing flexibility
    /// </summary>
    public async Task<Result<PaymentResult>> ProcessPaymentWithAutoGatewayAsync(
        decimal amount,
        string currency,
        string paymentMethodId,
        string description)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            var bestGatewayResult = await FindBestGatewayForCurrencyAsync(currency);
            if (bestGatewayResult.IsFailure)
            {
                return Result<PaymentResult>.Failure(bestGatewayResult.Error);
            }

            return await ProcessPaymentAsync(
                bestGatewayResult.Value, 
                amount, 
                currency, 
                paymentMethodId, 
                description);
        }, "ProcessPaymentWithAutoGateway");
    }
}

/// <summary>
/// Information about a payment gateway
/// </summary>
public class PaymentGatewayInfo
{
    public string Name { get; set; } = string.Empty;
    public List<string> SupportedCurrencies { get; set; } = new();
} 
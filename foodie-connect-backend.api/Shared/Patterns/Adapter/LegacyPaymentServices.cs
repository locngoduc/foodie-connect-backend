namespace foodie_connect_backend.Shared.Patterns.Adapter;

/// <summary>
/// Legacy Stripe-like payment service with incompatible interface
/// This represents the "Adaptee" in the Adapter pattern
/// </summary>
public class LegacyStripeService
{
    private readonly ILogger<LegacyStripeService> _logger;

    public LegacyStripeService(ILogger<LegacyStripeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Legacy method with different parameter structure
    /// </summary>
    public async Task<StripeChargeResponse> CreateChargeAsync(StripeChargeRequest request)
    {
        _logger.LogInformation("Processing Stripe charge for amount: {Amount}", request.AmountInCents);
        
        // Simulate API call delay
        await Task.Delay(100);
        
        return new StripeChargeResponse
        {
            Id = $"ch_{Guid.NewGuid().ToString("N")[..24]}",
            Amount = request.AmountInCents,
            Currency = request.Currency.ToLower(),
            Status = "succeeded",
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    public async Task<StripeRefundResponse> CreateRefundAsync(string chargeId, int? amountInCents = null)
    {
        _logger.LogInformation("Processing Stripe refund for charge: {ChargeId}", chargeId);
        
        await Task.Delay(50);
        
        return new StripeRefundResponse
        {
            Id = $"re_{Guid.NewGuid().ToString("N")[..24]}",
            ChargeId = chargeId,
            Amount = amountInCents ?? 1000,
            Status = "succeeded",
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    public async Task<StripeChargeResponse> RetrieveChargeAsync(string chargeId)
    {
        _logger.LogInformation("Retrieving Stripe charge: {ChargeId}", chargeId);
        
        await Task.Delay(50);
        
        return new StripeChargeResponse
        {
            Id = chargeId,
            Amount = 1000,
            Currency = "usd",
            Status = "succeeded",
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}

/// <summary>
/// Legacy PayPal-like payment service with different interface structure
/// </summary>
public class LegacyPayPalService
{
    private readonly ILogger<LegacyPayPalService> _logger;

    public LegacyPayPalService(ILogger<LegacyPayPalService> logger)
    {
        _logger = logger;
    }

    public async Task<PayPalPaymentResponse> ExecutePaymentAsync(PayPalPaymentRequest payment)
    {
        _logger.LogInformation("Processing PayPal payment for {Amount} {Currency}", payment.Total, payment.CurrencyCode);
        
        await Task.Delay(150);
        
        return new PayPalPaymentResponse
        {
            PaymentId = $"PAY-{Guid.NewGuid().ToString().ToUpper()}",
            State = "approved",
            Total = payment.Total,
            CurrencyCode = payment.CurrencyCode,
            CreateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }

    public async Task<PayPalRefundResponse> RefundPaymentAsync(string paymentId, decimal amount, string currencyCode)
    {
        _logger.LogInformation("Processing PayPal refund for payment: {PaymentId}", paymentId);
        
        await Task.Delay(100);
        
        return new PayPalRefundResponse
        {
            RefundId = $"REF-{Guid.NewGuid().ToString().ToUpper()}",
            ParentPayment = paymentId,
            Amount = amount,
            CurrencyCode = currencyCode,
            State = "completed",
            CreateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }

    public async Task<PayPalPaymentResponse> GetPaymentDetailsAsync(string paymentId)
    {
        _logger.LogInformation("Retrieving PayPal payment: {PaymentId}", paymentId);
        
        await Task.Delay(75);
        
        return new PayPalPaymentResponse
        {
            PaymentId = paymentId,
            State = "approved",
            Total = 10.00m,
            CurrencyCode = "USD",
            CreateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }
}

/// <summary>
/// Legacy Square-like payment service
/// </summary>
public class LegacySquareService
{
    private readonly ILogger<LegacySquareService> _logger;

    public LegacySquareService(ILogger<LegacySquareService> logger)
    {
        _logger = logger;
    }

    public async Task<SquarePaymentResult> ProcessPaymentAsync(SquarePaymentData data)
    {
        _logger.LogInformation("Processing Square payment for {Amount} cents", data.AmountMoney.Amount);
        
        await Task.Delay(120);
        
        return new SquarePaymentResult
        {
            PaymentId = Guid.NewGuid().ToString(),
            Status = "COMPLETED",
            AmountMoney = data.AmountMoney,
            CreatedAt = DateTime.UtcNow.ToString("O")
        };
    }

    public async Task<SquareRefundResult> CreateRefundAsync(string paymentId, SquareMoneyAmount amount)
    {
        _logger.LogInformation("Processing Square refund for payment: {PaymentId}", paymentId);
        
        await Task.Delay(90);
        
        return new SquareRefundResult
        {
            RefundId = Guid.NewGuid().ToString(),
            PaymentId = paymentId,
            Status = "COMPLETED",
            AmountMoney = amount,
            CreatedAt = DateTime.UtcNow.ToString("O")
        };
    }

    public async Task<SquarePaymentResult> GetPaymentAsync(string paymentId)
    {
        _logger.LogInformation("Retrieving Square payment: {PaymentId}", paymentId);
        
        await Task.Delay(60);
        
        return new SquarePaymentResult
        {
            PaymentId = paymentId,
            Status = "COMPLETED",
            AmountMoney = new SquareMoneyAmount { Amount = 1000, Currency = "USD" },
            CreatedAt = DateTime.UtcNow.ToString("O")
        };
    }
}

// Stripe DTOs
public class StripeChargeRequest
{
    public int AmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class StripeChargeResponse
{
    public string Id { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long Created { get; set; }
}

public class StripeRefundResponse
{
    public string Id { get; set; } = string.Empty;
    public string ChargeId { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public long Created { get; set; }
}

// PayPal DTOs
public class PayPalPaymentRequest
{
    public decimal Total { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PayPalPaymentResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string CreateTime { get; set; } = string.Empty;
}

public class PayPalRefundResponse
{
    public string RefundId { get; set; } = string.Empty;
    public string ParentPayment { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string CreateTime { get; set; } = string.Empty;
}

// Square DTOs
public class SquarePaymentData
{
    public SquareMoneyAmount AmountMoney { get; set; } = new();
    public string SourceId { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public class SquarePaymentResult
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public SquareMoneyAmount AmountMoney { get; set; } = new();
    public string CreatedAt { get; set; } = string.Empty;
}

public class SquareRefundResult
{
    public string RefundId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public SquareMoneyAmount AmountMoney { get; set; } = new();
    public string CreatedAt { get; set; } = string.Empty;
}

public class SquareMoneyAmount
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
} 
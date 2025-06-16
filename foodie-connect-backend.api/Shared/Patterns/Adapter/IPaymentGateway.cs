using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Adapter;

/// <summary>
/// Target interface for payment processing in the Adapter pattern
/// All payment gateway adapters must implement this interface
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Process a payment through the payment gateway
    /// </summary>
    /// <param name="amount">Payment amount in cents</param>
    /// <param name="currency">Currency code (e.g., USD, EUR)</param>
    /// <param name="paymentMethodId">Payment method identifier</param>
    /// <param name="description">Payment description</param>
    /// <returns>Payment result with transaction ID</returns>
    Task<Result<PaymentResult>> ProcessPaymentAsync(
        decimal amount, 
        string currency, 
        string paymentMethodId, 
        string description);

    /// <summary>
    /// Refund a payment
    /// </summary>
    /// <param name="transactionId">Original transaction ID</param>
    /// <param name="amount">Refund amount in cents (null for full refund)</param>
    /// <param name="reason">Refund reason</param>
    /// <returns>Refund result</returns>
    Task<Result<RefundResult>> RefundPaymentAsync(
        string transactionId, 
        decimal? amount = null, 
        string? reason = null);

    /// <summary>
    /// Check the status of a payment
    /// </summary>
    /// <param name="transactionId">Transaction ID to check</param>
    /// <returns>Payment status information</returns>
    Task<Result<PaymentStatus>> GetPaymentStatusAsync(string transactionId);

    /// <summary>
    /// Gateway name for identification
    /// </summary>
    string GatewayName { get; }

    /// <summary>
    /// Supported currencies by this gateway
    /// </summary>
    List<string> SupportedCurrencies { get; }
}

/// <summary>
/// Result of a payment operation
/// </summary>
public class PaymentResult
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string GatewayReference { get; set; } = string.Empty;
}

/// <summary>
/// Result of a refund operation
/// </summary>
public class RefundResult
{
    public string RefundId { get; set; } = string.Empty;
    public string OriginalTransactionId { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Payment status information
/// </summary>
public class PaymentStatus
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
} 
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Adapter;

/// <summary>
/// Adapter that adapts the LegacyPayPalService to the IPaymentGateway interface
/// This is a concrete adapter in the Adapter pattern
/// </summary>
public class PayPalPaymentAdapter : IPaymentGateway
{
    private readonly LegacyPayPalService _payPalService;

    public PayPalPaymentAdapter(LegacyPayPalService payPalService)
    {
        _payPalService = payPalService;
    }

    public string GatewayName => "PayPal";

    public List<string> SupportedCurrencies => new()
    {
        "USD", "EUR", "GBP", "CAD", "AUD", "JPY", "CHF", "BRL", "MXN", "INR"
    };

    public async Task<Result<PaymentResult>> ProcessPaymentAsync(
        decimal amount, 
        string currency, 
        string paymentMethodId, 
        string description)
    {
        try
        {
            if (!SupportedCurrencies.Contains(currency.ToUpper()))
            {
                return Result<PaymentResult>.Failure(
                    new AppError("UNSUPPORTED_CURRENCY", $"Currency {currency} is not supported by PayPal"));
            }

            var request = new PayPalPaymentRequest
            {
                Total = amount,
                CurrencyCode = currency.ToUpper(),
                PaymentMethod = paymentMethodId,
                Description = description
            };

            var payPalResponse = await _payPalService.ExecutePaymentAsync(request);

            // Adapt PayPal response to our common interface
            var result = new PaymentResult
            {
                TransactionId = payPalResponse.PaymentId,
                Amount = payPalResponse.Total,
                Currency = payPalResponse.CurrencyCode,
                Status = MapPayPalStatus(payPalResponse.State),
                ProcessedAt = DateTime.Parse(payPalResponse.CreateTime),
                GatewayReference = payPalResponse.PaymentId
            };

            return Result<PaymentResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PaymentResult>.Failure(
                new AppError("PAYPAL_PAYMENT_ERROR", $"PayPal payment failed: {ex.Message}"));
        }
    }

    public async Task<Result<RefundResult>> RefundPaymentAsync(
        string transactionId, 
        decimal? amount = null, 
        string? reason = null)
    {
        try
        {
            // PayPal requires explicit amount and currency for refunds
            // For demo purposes, we'll use a default if not provided
            var refundAmount = amount ?? 10.00m;
            var currency = "USD"; // In real implementation, would fetch from original transaction
            
            var payPalRefund = await _payPalService.RefundPaymentAsync(transactionId, refundAmount, currency);

            var result = new RefundResult
            {
                RefundId = payPalRefund.RefundId,
                OriginalTransactionId = payPalRefund.ParentPayment,
                RefundAmount = payPalRefund.Amount,
                Currency = payPalRefund.CurrencyCode,
                Status = MapPayPalStatus(payPalRefund.State),
                ProcessedAt = DateTime.Parse(payPalRefund.CreateTime)
            };

            return Result<RefundResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<RefundResult>.Failure(
                new AppError("PAYPAL_REFUND_ERROR", $"PayPal refund failed: {ex.Message}"));
        }
    }

    public async Task<Result<PaymentStatus>> GetPaymentStatusAsync(string transactionId)
    {
        try
        {
            var payPalPayment = await _payPalService.GetPaymentDetailsAsync(transactionId);

            var status = new PaymentStatus
            {
                TransactionId = payPalPayment.PaymentId,
                Status = MapPayPalStatus(payPalPayment.State),
                Amount = payPalPayment.Total,
                Currency = payPalPayment.CurrencyCode,
                CreatedAt = DateTime.Parse(payPalPayment.CreateTime),
                ProcessedAt = DateTime.Parse(payPalPayment.CreateTime),
                FailureReason = payPalPayment.State != "approved" ? "Payment not approved" : null
            };

            return Result<PaymentStatus>.Success(status);
        }
        catch (Exception ex)
        {
            return Result<PaymentStatus>.Failure(
                new AppError("PAYPAL_STATUS_ERROR", $"Failed to get PayPal payment status: {ex.Message}"));
        }
    }

    /// <summary>
    /// Maps PayPal-specific status values to standardized status values
    /// </summary>
    private static string MapPayPalStatus(string payPalState)
    {
        return payPalState.ToLower() switch
        {
            "approved" => "Completed",
            "created" => "Pending",
            "failed" => "Failed",
            "completed" => "Completed",
            "cancelled" => "Cancelled",
            _ => "Unknown"
        };
    }
} 
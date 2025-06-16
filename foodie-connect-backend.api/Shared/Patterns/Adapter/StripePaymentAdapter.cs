using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Adapter;

/// <summary>
/// Adapter that adapts the LegacyStripeService to the IPaymentGateway interface
/// This is a concrete adapter in the Adapter pattern
/// </summary>
public class StripePaymentAdapter : IPaymentGateway
{
    private readonly LegacyStripeService _stripeService;

    public StripePaymentAdapter(LegacyStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    public string GatewayName => "Stripe";

    public List<string> SupportedCurrencies => new()
    {
        "USD", "EUR", "GBP", "CAD", "AUD", "JPY", "CHF", "SEK", "NOK", "DKK"
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
                    new AppError("UNSUPPORTED_CURRENCY", $"Currency {currency} is not supported by Stripe"));
            }

            // Convert decimal dollars to cents (Stripe expects cents)
            var amountInCents = (int)(amount * 100);

            var request = new StripeChargeRequest
            {
                AmountInCents = amountInCents,
                Currency = currency.ToUpper(),
                Source = paymentMethodId,
                Description = description
            };

            var stripeResponse = await _stripeService.CreateChargeAsync(request);

            // Adapt Stripe response to our common interface
            var result = new PaymentResult
            {
                TransactionId = stripeResponse.Id,
                Amount = stripeResponse.Amount / 100m, // Convert cents back to dollars
                Currency = stripeResponse.Currency.ToUpper(),
                Status = MapStripeStatus(stripeResponse.Status),
                ProcessedAt = DateTimeOffset.FromUnixTimeSeconds(stripeResponse.Created).DateTime,
                GatewayReference = stripeResponse.Id
            };

            return Result<PaymentResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PaymentResult>.Failure(
                new AppError("STRIPE_PAYMENT_ERROR", $"Stripe payment failed: {ex.Message}"));
        }
    }

    public async Task<Result<RefundResult>> RefundPaymentAsync(
        string transactionId, 
        decimal? amount = null, 
        string? reason = null)
    {
        try
        {
            int? amountInCents = amount.HasValue ? (int)(amount.Value * 100) : null;
            
            var stripeRefund = await _stripeService.CreateRefundAsync(transactionId, amountInCents);

            var result = new RefundResult
            {
                RefundId = stripeRefund.Id,
                OriginalTransactionId = stripeRefund.ChargeId,
                RefundAmount = stripeRefund.Amount / 100m,
                Currency = "USD", // Stripe doesn't return currency in refund response
                Status = MapStripeStatus(stripeRefund.Status),
                ProcessedAt = DateTimeOffset.FromUnixTimeSeconds(stripeRefund.Created).DateTime
            };

            return Result<RefundResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<RefundResult>.Failure(
                new AppError("STRIPE_REFUND_ERROR", $"Stripe refund failed: {ex.Message}"));
        }
    }

    public async Task<Result<PaymentStatus>> GetPaymentStatusAsync(string transactionId)
    {
        try
        {
            var stripeCharge = await _stripeService.RetrieveChargeAsync(transactionId);

            var status = new PaymentStatus
            {
                TransactionId = stripeCharge.Id,
                Status = MapStripeStatus(stripeCharge.Status),
                Amount = stripeCharge.Amount / 100m,
                Currency = stripeCharge.Currency.ToUpper(),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stripeCharge.Created).DateTime,
                ProcessedAt = DateTimeOffset.FromUnixTimeSeconds(stripeCharge.Created).DateTime,
                FailureReason = stripeCharge.Status != "succeeded" ? "Payment failed" : null
            };

            return Result<PaymentStatus>.Success(status);
        }
        catch (Exception ex)
        {
            return Result<PaymentStatus>.Failure(
                new AppError("STRIPE_STATUS_ERROR", $"Failed to get Stripe payment status: {ex.Message}"));
        }
    }

    /// <summary>
    /// Maps Stripe-specific status values to standardized status values
    /// </summary>
    private static string MapStripeStatus(string stripeStatus)
    {
        return stripeStatus.ToLower() switch
        {
            "succeeded" => "Completed",
            "pending" => "Pending",
            "failed" => "Failed",
            _ => "Unknown"
        };
    }
} 
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Adapter;

/// <summary>
/// Adapter that adapts the LegacySquareService to the IPaymentGateway interface
/// This is a concrete adapter in the Adapter pattern
/// </summary>
public class SquarePaymentAdapter : IPaymentGateway
{
    private readonly LegacySquareService _squareService;

    public SquarePaymentAdapter(LegacySquareService squareService)
    {
        _squareService = squareService;
    }

    public string GatewayName => "Square";

    public List<string> SupportedCurrencies => new()
    {
        "USD", "CAD", "GBP", "EUR", "AUD", "JPY"
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
                    new AppError("UNSUPPORTED_CURRENCY", $"Currency {currency} is not supported by Square"));
            }

            // Square expects amounts in smallest currency unit (cents for USD)
            var amountInCents = (long)(amount * 100);

            var paymentData = new SquarePaymentData
            {
                AmountMoney = new SquareMoneyAmount
                {
                    Amount = amountInCents,
                    Currency = currency.ToUpper()
                },
                SourceId = paymentMethodId,
                Note = description
            };

            var squareResponse = await _squareService.ProcessPaymentAsync(paymentData);

            // Adapt Square response to our common interface
            var result = new PaymentResult
            {
                TransactionId = squareResponse.PaymentId,
                Amount = squareResponse.AmountMoney.Amount / 100m, // Convert cents back to dollars
                Currency = squareResponse.AmountMoney.Currency,
                Status = MapSquareStatus(squareResponse.Status),
                ProcessedAt = DateTime.Parse(squareResponse.CreatedAt),
                GatewayReference = squareResponse.PaymentId
            };

            return Result<PaymentResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PaymentResult>.Failure(
                new AppError("SQUARE_PAYMENT_ERROR", $"Square payment failed: {ex.Message}"));
        }
    }

    public async Task<Result<RefundResult>> RefundPaymentAsync(
        string transactionId, 
        decimal? amount = null, 
        string? reason = null)
    {
        try
        {
            // Square requires amount to be specified
            var refundAmount = amount ?? 10.00m;
            var amountInCents = (long)(refundAmount * 100);
            
            var refundAmountMoney = new SquareMoneyAmount
            {
                Amount = amountInCents,
                Currency = "USD" // In real implementation, would fetch from original transaction
            };
            
            var squareRefund = await _squareService.CreateRefundAsync(transactionId, refundAmountMoney);

            var result = new RefundResult
            {
                RefundId = squareRefund.RefundId,
                OriginalTransactionId = squareRefund.PaymentId,
                RefundAmount = squareRefund.AmountMoney.Amount / 100m,
                Currency = squareRefund.AmountMoney.Currency,
                Status = MapSquareStatus(squareRefund.Status),
                ProcessedAt = DateTime.Parse(squareRefund.CreatedAt)
            };

            return Result<RefundResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<RefundResult>.Failure(
                new AppError("SQUARE_REFUND_ERROR", $"Square refund failed: {ex.Message}"));
        }
    }

    public async Task<Result<PaymentStatus>> GetPaymentStatusAsync(string transactionId)
    {
        try
        {
            var squarePayment = await _squareService.GetPaymentAsync(transactionId);

            var status = new PaymentStatus
            {
                TransactionId = squarePayment.PaymentId,
                Status = MapSquareStatus(squarePayment.Status),
                Amount = squarePayment.AmountMoney.Amount / 100m,
                Currency = squarePayment.AmountMoney.Currency,
                CreatedAt = DateTime.Parse(squarePayment.CreatedAt),
                ProcessedAt = DateTime.Parse(squarePayment.CreatedAt),
                FailureReason = squarePayment.Status != "COMPLETED" ? "Payment not completed" : null
            };

            return Result<PaymentStatus>.Success(status);
        }
        catch (Exception ex)
        {
            return Result<PaymentStatus>.Failure(
                new AppError("SQUARE_STATUS_ERROR", $"Failed to get Square payment status: {ex.Message}"));
        }
    }

    /// <summary>
    /// Maps Square-specific status values to standardized status values
    /// </summary>
    private static string MapSquareStatus(string squareStatus)
    {
        return squareStatus.ToUpper() switch
        {
            "COMPLETED" => "Completed",
            "PENDING" => "Pending",
            "FAILED" => "Failed",
            "CANCELED" => "Cancelled",
            "APPROVED" => "Completed",
            _ => "Unknown"
        };
    }
} 
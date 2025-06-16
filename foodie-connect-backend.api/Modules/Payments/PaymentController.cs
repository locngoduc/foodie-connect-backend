using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Patterns.Adapter;
using foodie_connect_backend.Shared.Patterns.Decorator;

namespace foodie_connect_backend.Modules.Payments;

/// <summary>
/// Payment controller that demonstrates the Adapter pattern
/// Shows how different payment gateways can be used through a unified interface
/// </summary>
[ApiController]
[Route("v1/payments")]
[Tags("Payments - Adapter Pattern Demo")]
public class PaymentController : ControllerBase
{
    private readonly ILoggableService _paymentService;

    public PaymentController(ILoggableService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Process a payment using a specific gateway
    /// Demonstrates the Adapter pattern by allowing different payment gateways
    /// to be used through the same interface
    /// </summary>
    /// <param name="request">Payment request details</param>
    /// <returns>Payment result</returns>
    /// <response code="200">Payment processed successfully</response>
    /// <response code="400">Invalid request or payment failed</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost("process")]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest request)
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.ProcessPaymentAsync(
                request.Gateway,
                request.Amount,
                request.Currency,
                request.PaymentMethodId,
                request.Description);
        }, "ProcessPayment");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Process a payment with automatic gateway selection
    /// Demonstrates how the Adapter pattern enables smart gateway selection
    /// </summary>
    /// <param name="request">Auto payment request</param>
    /// <returns>Payment result with selected gateway</returns>
    /// <response code="200">Payment processed successfully</response>
    /// <response code="400">Invalid request or no suitable gateway found</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost("process-auto")]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<PaymentResult>> ProcessPaymentAuto([FromBody] AutoPaymentRequest request)
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.ProcessPaymentWithAutoGatewayAsync(
                request.Amount,
                request.Currency,
                request.PaymentMethodId,
                request.Description);
        }, "ProcessPaymentAuto");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Refund a payment using the specified gateway
    /// Shows how the Adapter pattern provides consistent refund operations
    /// across different payment providers
    /// </summary>
    /// <param name="request">Refund request details</param>
    /// <returns>Refund result</returns>
    /// <response code="200">Refund processed successfully</response>
    /// <response code="400">Invalid request or refund failed</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost("refund")]
    [ProducesResponseType(typeof(RefundResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<RefundResult>> RefundPayment([FromBody] RefundRequest request)
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.RefundPaymentAsync(
                request.Gateway,
                request.TransactionId,
                request.Amount,
                request.Reason);
        }, "RefundPayment");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get payment status from any gateway
    /// Demonstrates how the Adapter pattern normalizes status checking
    /// across different payment providers
    /// </summary>
    /// <param name="gateway">Payment gateway name</param>
    /// <param name="transactionId">Transaction ID to check</param>
    /// <returns>Payment status information</returns>
    /// <response code="200">Status retrieved successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("status/{gateway}/{transactionId}")]
    [ProducesResponseType(typeof(PaymentStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<PaymentStatus>> GetPaymentStatus(string gateway, string transactionId)
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.GetPaymentStatusAsync(gateway, transactionId);
        }, "GetPaymentStatus");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all available payment gateways and their capabilities
    /// Shows how the Adapter pattern allows querying capabilities
    /// across different payment providers
    /// </summary>
    /// <returns>List of available gateways</returns>
    /// <response code="200">Gateways retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("gateways")]
    [ProducesResponseType(typeof(List<PaymentGatewayInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<List<PaymentGatewayInfo>>> GetAvailableGateways()
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.GetAvailableGatewaysAsync();
        }, "GetAvailableGateways");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Find the best payment gateway for a specific currency
    /// Demonstrates how the Adapter pattern enables intelligent gateway selection
    /// </summary>
    /// <param name="currency">Currency code (e.g., USD, EUR)</param>
    /// <returns>Best gateway name for the currency</returns>
    /// <response code="200">Best gateway found</response>
    /// <response code="400">No gateway supports the currency</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("best-gateway/{currency}")]
    [ProducesResponseType(typeof(BestGatewayResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<BestGatewayResponse>> FindBestGateway(string currency)
    {
        var result = await _paymentService.ExecuteWithResultAsync(async () =>
        {
            var paymentService = _paymentService.DecoratedService as PaymentService
                ?? throw new InvalidOperationException("Expected PaymentService");

            return await paymentService.FindBestGatewayForCurrencyAsync(currency);
        }, "FindBestGateway");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new BestGatewayResponse { Gateway = result.Value, Currency = currency.ToUpper() });
    }
}

// Request/Response DTOs
public class PaymentRequest
{
    public string Gateway { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethodId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AutoPaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethodId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RefundRequest
{
    public string Gateway { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string? Reason { get; set; }
}

public class BestGatewayResponse
{
    public string Gateway { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
} 
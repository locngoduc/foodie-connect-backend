using System.Diagnostics;
using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Concrete decorator that adds logging functionality to services
/// Implements the Decorator pattern
/// </summary>
public class LoggingServiceDecorator : ILoggableService, IServiceDecorator<ILoggableService>
{
    private readonly ILoggableService _decoratedService;
    private readonly ILogger<LoggingServiceDecorator> _logger;

    public LoggingServiceDecorator(ILoggableService decoratedService, ILogger<LoggingServiceDecorator> logger)
    {
        _decoratedService = decoratedService;
        _logger = logger;
    }

    public ILoggableService DecoratedService => _decoratedService;

    public async Task<Result<T>> ExecuteWithResultAsync<T>(Func<Task<Result<T>>> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting operation: {OperationName}", operationName);

        try
        {
            var result = await _decoratedService.ExecuteWithResultAsync(operation, operationName);
            
            stopwatch.Stop();
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning("Operation {OperationName} failed in {ElapsedMs}ms. Error: {ErrorCode} - {ErrorMessage}", 
                    operationName, stopwatch.ElapsedMilliseconds, result.Error.Code, result.Error.Message);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Operation {OperationName} threw exception after {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task ExecuteAsync(Func<Task> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting operation: {OperationName}", operationName);

        try
        {
            await _decoratedService.ExecuteAsync(operation, operationName);
            
            stopwatch.Stop();
            _logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Operation {OperationName} threw exception after {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
} 
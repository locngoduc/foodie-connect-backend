using foodie_connect_backend.Modules.Dishes.Dtos;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace foodie_connect_backend.Modules.Dishes.Hub;

[SignalRHub]
public class DishViewersHub(ActiveDishViewersService viewersService, ILogger<DishViewersHub> logger)
    : Microsoft.AspNetCore.SignalR.Hub
{
    /// <summary>
    /// Start viewing a dish. Increments the viewer count and notifies all clients watching this dish.
    /// </summary>
    /// <param name="dishId"></param>
    public async Task StartViewingDish(Guid dishId)
    {
        try
        {
            var connectionId = Context.ConnectionId;
            var viewerCount = viewersService.StartViewing(dishId, connectionId);
            
            // Add to SignalR group for this dish
            await Groups.AddToGroupAsync(connectionId, dishId.ToString());
            
            // Notify all clients watching this dish about the updated count
            await Clients.Group(dishId.ToString()).SendAsync("ViewerCountUpdated", new DishViewerDto { DishId = dishId, ViewerCount = viewerCount});
            
            logger.LogInformation("User {ConnectionId} started viewing dish {DishId}. Total viewers: {Count}", 
                connectionId, dishId, viewerCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while starting to view dish {DishId}", dishId);
            throw;
        }
    }

    [SignalRHidden]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var connectionId = Context.ConnectionId;
            var affectedDishes = viewersService.RemoveViewerFromAllDishes(connectionId);

            // Notify all affected dish groups about updated counts
            foreach (var (dishId, viewerCount) in affectedDishes)
            {
                await Groups.RemoveFromGroupAsync(connectionId, dishId.ToString());
                await Clients.Group(dishId.ToString()).SendAsync("ViewerCountUpdated", dishId, viewerCount);
                
                logger.LogInformation("User {ConnectionId} disconnected from dish {DishId}. Remaining viewers: {Count}", 
                    connectionId, dishId, viewerCount);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during disconnect handling for connection {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}

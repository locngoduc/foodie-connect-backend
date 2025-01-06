using System.Web;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.RestaurantServices.Dtos;
using foodie_connect_backend.Modules.RestaurantServices.Mapper;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.RestaurantServices;

public class RestaurantServicesService (ApplicationDbContext dbContext)
{
    public async Task<Result<Service>> AddRestaurantService(Guid restaurantId, CreateRestaurantServiceDto serviceDto)
    {
        var restaurant = await dbContext.Restaurants.FirstOrDefaultAsync(res => res.Id == restaurantId);
        if (restaurant is null)
            return Result<Service>.Failure(RestaurantError.RestaurantNotExist());

        var serviceExists = await dbContext.Services.FirstOrDefaultAsync(ser => ser.Name == serviceDto.Name);
        if (serviceExists != null) 
        {
            return Result<Service>.Failure(RestaurantServiceError.ServiceConflict());
        }

        var formattedName = string.Join("-", serviceDto.Name.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
        var service = new Service
        {
            Name = formattedName,
            RestaurantId = restaurantId
        };

        dbContext.Services.Add(service);
        await dbContext.SaveChangesAsync();

        return Result<Service>.Success(restaurant.Services.Single(x=>x.Name==formattedName));
    }

    public async Task<Result<Service[]>> GetRestaurantServices(Guid restaurantId)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.Services)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
    
        // Checks
        if (restaurant == null) 
            return Result<Service[]>.Failure(RestaurantError.RestaurantNotExist());
        
        return Result<Service[]>.Success(restaurant.Services.ToArray());
    }

    public async Task<Result<Service>> RenameRestaurantService(Guid restaurantId, string oldName, string newName)
    {
        var formattedNewName = string.Join("-", newName.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.Services)
            .FirstOrDefaultAsync(res => res.Id == restaurantId);
        
        if (restaurant == null) 
            return Result<Service>.Failure(RestaurantError.RestaurantNotExist());
        if(restaurant.Services.All(x=>x.Name!=oldName)) 
            return Result<Service>.Failure(RestaurantServiceError.ServiceNotFound());
        if(restaurant.Services.Any(x=>x.Name==formattedNewName)) 
            return Result<Service>.Failure(RestaurantServiceError.ServiceConflict());

        var newService = new Service { Name = formattedNewName, RestaurantId = restaurantId};
        var oldService = dbContext.Services.Single(x => x.Name == oldName);
        dbContext.Services.Add(newService);
        dbContext.Services.Remove(oldService);
        await dbContext.SaveChangesAsync();

        return Result<Service>.Success(restaurant.Services.Single(x=>x.Name==formattedNewName));
    }

    public async Task<Result<Service>> DeleteRestaurantService(Guid restaurantId, string serviceName)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.Services)
            .FirstOrDefaultAsync(res => res.Id == restaurantId);
        
        if (restaurant == null) 
            return Result<Service>.Failure(RestaurantError.RestaurantNotExist());
        if(restaurant.Services.All(x=>x.Name!=serviceName)) 
            return Result<Service>.Failure(RestaurantServiceError.ServiceNotFound());

        var serviceToBeDeleted = dbContext.Services.Single(x => x.Name == serviceName);
        dbContext.Services.Remove(serviceToBeDeleted);
        await dbContext.SaveChangesAsync();

        return Result<Service>.Success(serviceToBeDeleted);
    }
}
using foodie_connect_backend.Area.Dtos;
using foodie_connect_backend.Data;
using foodie_connect_backend.GeoCoder;
using foodie_connect_backend.Restaurants;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace foodie_connect_backend.Areas;

public class AreasService(
    RestaurantsService restaurantsService,
    IGeoCoderService geoCoderService,
    ApplicationDbContext dbContext)
{
    public async Task<Result<Data.Area>> CreateArea(CreateAreaDto createAreaDto, string userId)
    {
        try
        {
            var restaurant = await dbContext.Restaurants.FindAsync(createAreaDto.RestaurantId);
            if(restaurant == null)
                return Result<Data.Area>.Failure(AppError.RecordNotFound("No restaurant is associated with this restaurantId."));
            if(restaurant.HeadId != userId)
                return Result<Data.Area>.Failure(AppError.InternalError("User does not belong to this restaurant."));
            
            var coordinates = createAreaDto.LatitudeLongitude.Split(',');
            if (coordinates.Length != 2 || 
                !double.TryParse(coordinates[0], out double latitude) ||
                !double.TryParse(coordinates[1], out double longitude))
            {
                return Result<Data.Area>.Failure(AppError.Conflict("Invalid coordinates."));
            }

            var resultArea = await geoCoderService.GetAddressAsync(latitude,longitude);
            if(resultArea.IsFailure)
                return Result<Data.Area>.Failure(AppError.InternalError("Error occured while creating a new area."));
            await dbContext.Areas.AddAsync(resultArea.Value);
            
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            restaurant.Location = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
            
            await dbContext.SaveChangesAsync(); 
            return Result<Data.Area>.Success(resultArea.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); 
            return Result<Data.Area>.Failure(AppError.InternalError("Failed to create area"));
        }
    }
}
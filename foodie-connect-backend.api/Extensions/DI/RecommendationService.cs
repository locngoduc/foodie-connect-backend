using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Newtonsoft.Json;
using RestSharp;

namespace foodie_connect_backend.Extensions.DI;

public class RecommendationService
{
    private readonly string recommenderUrl;   
    public RecommendationService(string recommenderUrl)
    {
        this.recommenderUrl = recommenderUrl;
    }

    public async Task<Result<List<string>>> GetRecommendedDishIdsAsync(string userId, int n = 10)
    {
        try
        {
            var client = new RestClient(recommenderUrl);
            var request = new RestRequest("recommended-dishes").AddParameter("userId",userId).AddParameter("n",n);
            var response = await client.GetAsync(request);
        
            if (response?.Content == null)
            {
                return Result<List<string>>.Failure(AppError.InternalError("Empty response received"));
            }

            var dishIds = JsonConvert.DeserializeObject<List<string>>(response.Content) 
                                ?? new List<string>();
        
            return Result<List<string>>.Success(dishIds);
        }
        catch (Exception ex)
        {
            return Result<List<string>>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<List<string>>> GetRecommendedRestaurantIdsAsync(string userId, int n = 10)
    {
        try
        {
            var client = new RestClient(recommenderUrl);
            var request = new RestRequest("recommended-restaurants").AddParameter("userId",userId).AddParameter("n",n);
            var response = await client.GetAsync(request);
        
            if (response?.Content == null)
            {
                return Result<List<string>>.Failure(AppError.InternalError("Empty response received"));
            }

            var restaurantIds = JsonConvert.DeserializeObject<List<string>>(response.Content) 
                                ?? new List<string>();
        
            return Result<List<string>>.Success(restaurantIds);
        }
        catch (Exception ex)
        {
            return Result<List<string>>.Failure(AppError.InternalError());
        }
    }
}
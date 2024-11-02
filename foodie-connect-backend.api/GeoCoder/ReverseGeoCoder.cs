using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;
using AreaModel = foodie_connect_backend.Data.Area;

namespace foodie_connect_backend.GeoCoder;

public class ReverseGeoCoder : IGeoCoderService
{
    private readonly string _apiKey;

    public ReverseGeoCoder(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<Result<AreaModel>> GetAddressAsync(double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(this._apiKey))
            return Result<AreaModel>.Failure(AppError.InternalError("Google API Key is missing"));
        
        var request = new LocationGeocodeRequest
        {
            Key = _apiKey,
            Location = new GeocodeRequestCoordinate(latitude, longitude)
        };

        try
        {
            var response = await GoogleMaps.Geocode.LocationGeocode.QueryAsync(request);

            if (response.Status == Status.Ok && response.Results.Any())
            {
                var area = ConvertToArea(response.Results.First());
                return Result<AreaModel>.Success(area);
            }
                
            
            return Result<AreaModel>.Failure(AppError.InternalError("Google API returned error"));
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during geocoding: {ex.Message}");
            return Result<AreaModel>.Failure(AppError.InternalError("Google API returned error"));
        }
    }

    private AreaModel ConvertToArea(GeocodingResult result)
{
    var area = new AreaModel
    {
        FormattedAddress = result.FormattedAddress,
    };

    foreach (var component in result.AddressComponents)
    {
        foreach (var type in component.Types)
        {
            switch (type)
            {
                case AddressComponentType.Street_Address:
                    area.StreetAddress = component.LongName;
                    break;

                case AddressComponentType.Route:
                    area.Route = component.LongName;
                    break;

                case AddressComponentType.Intersection:
                    area.Intersection = component.LongName;
                    break;

                case AddressComponentType.Political:
                    area.PoliticalEntity = component.LongName;
                    break;

                case AddressComponentType.Locality:
                    area.Locality = component.LongName;
                    break;

                case AddressComponentType.Administrative_Area_Level_1:
                    area.AdministrativeAreaLevel1 = component.LongName; // State or province
                    break;

                case AddressComponentType.Administrative_Area_Level_2:
                    area.AdministrativeAreaLevel2 = component.LongName; // County or district
                    break;

                case AddressComponentType.Administrative_Area_Level_3:
                    area.AdministrativeAreaLevel3 = component.LongName; // Minor civil division
                    break;

                case AddressComponentType.Sublocality:
                    area.Sublocality = component.LongName;
                    break;

                case AddressComponentType.Neighborhood:
                    area.Neighborhood = component.LongName;
                    break;

                case AddressComponentType.Premise:
                    area.Premise = component.LongName;
                    break;

                case AddressComponentType.Subpremise:
                    area.Subpremise = component.LongName;
                    break;

                case AddressComponentType.Postal_Code:
                    area.PostalCode = component.LongName; 
                    break;

                case AddressComponentType.Plus_Code:
                    area.PlusCode = component.LongName; 
                    break;

                case AddressComponentType.Natural_Feature:
                    area.NaturalFeature = component.LongName; 
                    break;

                case AddressComponentType.Airport:
                    area.Airport = component.LongName; 
                    break;

                case AddressComponentType.Park:
                    area.Park = component.LongName;
                    break;

                case AddressComponentType.Point_Of_Interest:
                    area.PointOfInterest = component.LongName;
                    break;

                case AddressComponentType.Country:
                    area.Country = component.LongName; 
                    break;
            }
        }
    }

    return area;
}

}
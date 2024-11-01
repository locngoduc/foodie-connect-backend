using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;

namespace foodie_connect_backend.GeoCoder;

public class ReverseGeoCoder : IGeoCoderService
{
    private readonly string _apiKey;

    public ReverseGeoCoder(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            Console.WriteLine("Api key is null");
        _apiKey = apiKey;
    }

    public async Task<GeocodingResult> GetAddressAsync(string latitudeLongitude)
    {
        var request = new LocationGeocodeRequest
        {
            Key = _apiKey,
            Location = new GeocodeRequestCoordinate(10.869956207608139, 106.80303263791059)
        };

        try
        {
            var response = await GoogleMaps.Geocode.LocationGeocode.QueryAsync(request);

            if (response.Status == Status.Ok && response.Results.Any())
            {
                foreach (var component in response.Results.First().AddressComponents)
                {
                    Console.WriteLine($"- Type: {string.Join(", ", component.Types)}");
                    Console.WriteLine($"  Long Name: {component.LongName}");
                    Console.WriteLine($"  Short Name: {component.ShortName}");
                }

                return response.Results.First();
            }

            Console.WriteLine($"Geocoding failed with status: {response.Status}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during geocoding: {ex.Message}");
            return null;
        }
    }
}
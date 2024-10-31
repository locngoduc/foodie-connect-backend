namespace foodie_connect_backend.GeoCoder;

public interface IGeoCoderService
{
    Task<GeocodingResult> GetAddressAsync(string latitudeLongitude);
}
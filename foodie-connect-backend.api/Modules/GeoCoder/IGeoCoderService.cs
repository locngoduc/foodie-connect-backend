using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Modules.GeoCoder;

public interface IGeoCoderService
{
    Task<Result<Data.Area>> GetAddressAsync(double latitude, double longitude);
}
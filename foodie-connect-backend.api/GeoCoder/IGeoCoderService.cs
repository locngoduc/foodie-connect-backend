using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using Area = foodie_connect_backend.Data.Area;

namespace foodie_connect_backend.GeoCoder;

public interface IGeoCoderService
{
    Task<Result<Data.Area>> GetAddressAsync(double latitude, double longitude);
}
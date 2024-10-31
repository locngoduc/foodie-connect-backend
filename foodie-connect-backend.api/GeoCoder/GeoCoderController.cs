using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.GeoCoder;

[Route("v1/geocoder")]
[ApiController]
public class GeoCoderController(IGeoCoderService geoCoderService) : ControllerBase
{
    [HttpGet("test")]
    public async Task<GeocodingResult> Test()
    {
        var result = await geoCoderService.GetAddressAsync("123");
        return result;
    }
}
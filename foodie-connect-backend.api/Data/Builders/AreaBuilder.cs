using System;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Data.Builders;

public class AreaBuilder
{
    private readonly Area _area = new();

    public AreaBuilder WithId(Guid id) { _area.Id = id; return this; }
    public AreaBuilder WithFormattedAddress(string address) { _area.FormattedAddress = address; return this; }
    public AreaBuilder WithStreetAddress(string? street) { _area.StreetAddress = street; return this; }
    public AreaBuilder WithRoute(string? route) { _area.Route = route; return this; }
    public AreaBuilder WithIntersection(string? intersection) { _area.Intersection = intersection; return this; }
    public AreaBuilder WithPoliticalEntity(string? entity) { _area.PoliticalEntity = entity; return this; }
    public AreaBuilder WithCountry(string country) { _area.Country = country; return this; }
    public AreaBuilder WithAdministrativeAreaLevel1(string? val) { _area.AdministrativeAreaLevel1 = val; return this; }
    public AreaBuilder WithAdministrativeAreaLevel2(string? val) { _area.AdministrativeAreaLevel2 = val; return this; }
    public AreaBuilder WithAdministrativeAreaLevel3(string? val) { _area.AdministrativeAreaLevel3 = val; return this; }
    public AreaBuilder WithLocality(string? val) { _area.Locality = val; return this; }
    public AreaBuilder WithSublocality(string? val) { _area.Sublocality = val; return this; }
    public AreaBuilder WithNeighborhood(string? val) { _area.Neighborhood = val; return this; }
    public AreaBuilder WithPremise(string? val) { _area.Premise = val; return this; }
    public AreaBuilder WithSubpremise(string? val) { _area.Subpremise = val; return this; }
    public Area Build() => _area;
}

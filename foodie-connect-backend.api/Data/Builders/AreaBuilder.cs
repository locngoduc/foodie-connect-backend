using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class AreaBuilder
{
    private string _formattedAddress = null!;
    private string? _streetAddress;
    private string? _route;
    private string? _intersection;
    private string? _politicalEntity;
    private string _country = null!;
    private string? _administrativeAreaLevel1;
    private string? _administrativeAreaLevel2;
    private string? _administrativeAreaLevel3;
    private string? _locality;
    private string? _sublocality;
    private string? _neighborhood;
    private string? _premise;
    private string? _subpremise;
    public AreaBuilder WithFormattedAddress(string address) { _formattedAddress = address; return this; }
    public AreaBuilder WithStreetAddress(string? street) { _streetAddress = street; return this; }
    public AreaBuilder WithRoute(string? route) { _route = route; return this; }
    public AreaBuilder WithIntersection(string? intersection) { _intersection = intersection; return this; }
    public AreaBuilder WithPoliticalEntity(string? entity) { _politicalEntity = entity; return this; }
    public AreaBuilder WithCountry(string country) { _country = country; return this; }
    public AreaBuilder WithAdministrativeAreaLevel1(string? val) { _administrativeAreaLevel1 = val; return this; }
    public AreaBuilder WithAdministrativeAreaLevel2(string? val) { _administrativeAreaLevel2 = val; return this; }
    public AreaBuilder WithAdministrativeAreaLevel3(string? val) { _administrativeAreaLevel3 = val; return this; }
    public AreaBuilder WithLocality(string? val) { _locality = val; return this; }
    public AreaBuilder WithSublocality(string? val) { _sublocality = val; return this; }
    public AreaBuilder WithNeighborhood(string? val) { _neighborhood = val; return this; }
    public AreaBuilder WithPremise(string? val) { _premise = val; return this; }
    public AreaBuilder WithSubpremise(string? val) { _subpremise = val; return this; }
    public Area Build() => new Area
    {
        FormattedAddress = _formattedAddress,
        StreetAddress = _streetAddress,
        Route = _route,
        Intersection = _intersection,
        PoliticalEntity = _politicalEntity,
        Country = _country,
        AdministrativeAreaLevel1 = _administrativeAreaLevel1,
        AdministrativeAreaLevel2 = _administrativeAreaLevel2,
        AdministrativeAreaLevel3 = _administrativeAreaLevel3,
        Locality = _locality,
        Sublocality = _sublocality,
        Neighborhood = _neighborhood,
        Premise = _premise,
        Subpremise = _subpremise
    };
}

using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Enums;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class RestaurantBuilder: IBuilder<Restaurant>
{
    private Guid _id;
    private string _name = null!;
    private TimeOnly _openTime;
    private TimeOnly _closeTime;
    private RestaurantStatus _status = RestaurantStatus.Open;
    private ICollection<SocialLink> _socialLinks = new List<SocialLink>();
    private string _phone = null!;
    private IList<string> _images = new List<string>();
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private Guid _areaId;
    private bool _isDeleted;
    private ICollection<DishCategory> _dishCategories = new List<DishCategory>();

    public RestaurantBuilder WithId(Guid id) { _id = id; return this; }
    public RestaurantBuilder WithName(string name) { _name = name; return this; }
    public RestaurantBuilder WithOpenTime(TimeOnly openTime) { _openTime = openTime; return this; }
    public RestaurantBuilder WithCloseTime(TimeOnly closeTime) { _closeTime = closeTime; return this; }
    public RestaurantBuilder WithStatus(RestaurantStatus status) { _status = status; return this; }
    public RestaurantBuilder WithSocialLinks(ICollection<SocialLink> links) { _socialLinks = links; return this; }
    public RestaurantBuilder WithPhone(string phone) { _phone = phone; return this; }
    public RestaurantBuilder WithImages(IList<string> images) { _images = images; return this; }
    public RestaurantBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }
    public RestaurantBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public RestaurantBuilder WithAreaId(Guid areaId) { _areaId = areaId; return this; }
    public RestaurantBuilder WithIsDeleted(bool isDeleted) { _isDeleted = isDeleted; return this; }
    public RestaurantBuilder WithDishCategories(ICollection<DishCategory> categories) { _dishCategories = categories; return this; }
    public Restaurant Build() => new Restaurant
    {
        Id = _id,
        Name = _name,
        OpenTime = _openTime,
        CloseTime = _closeTime,
        Status = _status,
        SocialLinks = _socialLinks,
        Phone = _phone,
        Images = _images,
        CreatedAt = _createdAt,
        UpdatedAt = _updatedAt,
        AreaId = _areaId,
        IsDeleted = _isDeleted,
        DishCategories = _dishCategories
    };
}

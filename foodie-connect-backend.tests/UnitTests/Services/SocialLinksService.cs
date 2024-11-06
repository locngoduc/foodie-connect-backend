using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Socials;
using foodie_connect_backend.Modules.Socials.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace food_connect_backend.tests.UnitTests.Services;

public class SocialsServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _dbContext;
    private readonly SocialsService _socialsService;

    public SocialsServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(_options);
        _socialsService = new SocialsService(_dbContext);
    }

    private async Task ClearDatabase()
    {
        _dbContext.Restaurants.RemoveRange(_dbContext.Restaurants);
        _dbContext.SocialLinks.RemoveRange(_dbContext.SocialLinks);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetRestaurantSocialLinks_WithValidId_ReturnsLinks()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>
            {
                new() { Id = "1", PlatformType = SocialPlatformType.Facebook, Url = "fb.com/test", RestaurantId = restaurantId },
                new() { Id = "2", PlatformType = SocialPlatformType.Twitter, Url = "instagram.com/test", RestaurantId = restaurantId }
            },
            Location = new Point(0,0)
        };

        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _socialsService.GetRestaurantSocialLinksAsync(restaurantId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(SocialPlatformType.Facebook, result.Value[0].PlatformType);
        Assert.Equal(SocialPlatformType.Twitter, result.Value[1].PlatformType);
    }

    [Fact]
    public async Task GetRestaurantSocialLinks_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "invalid";
    
        // Act
        var result = await _socialsService.GetRestaurantSocialLinksAsync(restaurantId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(SocialError.RestaurantNotFoundCode, result.Error.Code);
    }
    
    [Fact]
    public async Task AddSocialLink_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>(),
            Location = new Point(0,0)
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    
        var createDto = new CreateSocialDto
        {
            PlatformType = SocialPlatformType.Twitter,
            Url = "twitter.com/test"
        };
    
        // Act
        var result = await _socialsService.AddSocialLinkAsync(restaurantId, createDto);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SocialPlatformType.Twitter, result.Value.PlatformType);
        Assert.Equal("twitter.com/test", result.Value.Url);
        
        var savedRestaurant = await _dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstAsync(r => r.Id == restaurantId);
        Assert.Single(savedRestaurant.SocialLinks);
        Assert.Equal(SocialPlatformType.Twitter, savedRestaurant.SocialLinks.First().PlatformType);
    }
    
    [Fact]
    public async Task AddSocialLink_WithDuplicatePlatform_ReturnsFailure()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>
            {
                new() { Id = "1", PlatformType = SocialPlatformType.Twitter, Url = "twitter.com/existing", RestaurantId = restaurantId }
            },
            Location = new Point(0,0)
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        var createDto = new CreateSocialDto
        {
            PlatformType = SocialPlatformType.Twitter,
            Url = "twitter.com/test"
        };
    
        // Act
        var result = await _socialsService.AddSocialLinkAsync(restaurantId, createDto);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(SocialError.SocialAlreadyExistCode, result.Error.Code);
    }
    
    [Fact]
    public async Task UpdateSocialLink_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var socialLinkId = "link123";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>
            {
                new() { Id = socialLinkId, PlatformType = SocialPlatformType.Twitter, Url = "twitter.com/old", RestaurantId = restaurantId }
            },
            Location = new Point(0,0)
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
        
        var updateDto = new UpdateSocialDto
        {
            Id = socialLinkId,
            PlatformType = SocialPlatformType.Facebook,
            Url = "facebook.com/new"
        };
    
        // Act
        var result = await _socialsService.UpdateSocialLinkAsync(restaurantId, updateDto);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SocialPlatformType.Facebook, result.Value.PlatformType);
        Assert.Equal("facebook.com/new", result.Value.Url);
    
        var updatedRestaurant = await _dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstAsync(r => r.Id == restaurantId);
        Assert.Single(updatedRestaurant.SocialLinks);
        Assert.Equal(SocialPlatformType.Facebook, updatedRestaurant.SocialLinks.First().PlatformType);
    }
    
    [Fact]
    public async Task DeleteSocialLink_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var socialLinkId = "link123";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>
            {
                new() { Id = socialLinkId, PlatformType = SocialPlatformType.Twitter, Url = "twitter.com/test", RestaurantId = restaurantId }
            },
            Location = new Point(0,0)
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        // Act
        var result = await _socialsService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    
        var updatedRestaurant = await _dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstAsync(r => r.Id == restaurantId);
        Assert.Empty(updatedRestaurant.SocialLinks);
    }
    
    [Fact]
    public async Task DeleteSocialLink_WithInvalidRestaurant_ReturnsFailure()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "invalid";
        var socialLinkId = "link123";
    
        // Act
        var result = await _socialsService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(SocialError.RestaurantNotFoundCode, result.Error.Code);
    }
    
    [Fact]
    public async Task DeleteSocialLink_WithInvalidSocialLinkId_ReturnsFailure()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "rest123";
        var socialLinkId = "invalid";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Phone = "1234567890",
            HeadId = "head-user",
            SocialLinks = new List<SocialLink>(),
            Location = new Point(0,0)
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        // Act
        var result = await _socialsService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(SocialError.SocialDoesNotExistCode, result.Error.Code);
    }
}
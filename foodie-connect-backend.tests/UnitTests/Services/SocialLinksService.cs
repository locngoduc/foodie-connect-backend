using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Enums;
using foodie_connect_backend.SocialLinks;
using foodie_connect_backend.SocialLinks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace food_connect_backend.tests.UnitTests.Services;

public class SocialLinksServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _dbContext;
    private readonly SocialLinksService _socialLinksService;

    public SocialLinksServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(_options);
        _socialLinksService = new SocialLinksService(_dbContext);
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
                new() { Id = "1", Platform = SocialMediaPlatform.Facebook, Url = "fb.com/test", RestaurantId = restaurantId },
                new() { Id = "2", Platform = SocialMediaPlatform.Twitter, Url = "instagram.com/test", RestaurantId = restaurantId }
            }
        };

        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _socialLinksService.GetRestaurantSocialLinksAsync(restaurantId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(SocialMediaPlatform.Facebook, result.Value[0].Platform);
        Assert.Equal(SocialMediaPlatform.Twitter, result.Value[1].Platform);
    }

    [Fact]
    public async Task GetRestaurantSocialLinks_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        await ClearDatabase();
        var restaurantId = "invalid";
    
        // Act
        var result = await _socialLinksService.GetRestaurantSocialLinksAsync(restaurantId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Restaurant not found", result.Error!.Message);
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
            SocialLinks = new List<SocialLink>()
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    
        var createDto = new CreateSocialLinkDto
        {
            Platform = SocialMediaPlatform.Twitter,
            Url = "twitter.com/test"
        };
    
        // Act
        var result = await _socialLinksService.AddSocialLinkAsync(restaurantId, createDto);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SocialMediaPlatform.Twitter, result.Value.Platform);
        Assert.Equal("twitter.com/test", result.Value.Url);
        
        var savedRestaurant = await _dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstAsync(r => r.Id == restaurantId);
        Assert.Single(savedRestaurant.SocialLinks);
        Assert.Equal(SocialMediaPlatform.Twitter, savedRestaurant.SocialLinks.First().Platform);
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
                new() { Id = "1", Platform = SocialMediaPlatform.Twitter, Url = "twitter.com/existing", RestaurantId = restaurantId }
            }
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        var createDto = new CreateSocialLinkDto
        {
            Platform = SocialMediaPlatform.Twitter,
            Url = "twitter.com/test"
        };
    
        // Act
        var result = await _socialLinksService.AddSocialLinkAsync(restaurantId, createDto);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Social link for Twitter already exists", result.Error!.Message);
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
                new() { Id = socialLinkId, Platform = SocialMediaPlatform.Twitter, Url = "twitter.com/old", RestaurantId = restaurantId }
            }
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
        
        var updateDto = new UpdateSocialLinkDto
        {
            Id = socialLinkId,
            Platform = SocialMediaPlatform.Facebook,
            Url = "facebook.com/new"
        };
    
        // Act
        var result = await _socialLinksService.UpdateSocialLinkAsync(restaurantId, updateDto);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SocialMediaPlatform.Facebook, result.Value.Platform);
        Assert.Equal("facebook.com/new", result.Value.Url);
    
        var updatedRestaurant = await _dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstAsync(r => r.Id == restaurantId);
        Assert.Single(updatedRestaurant.SocialLinks);
        Assert.Equal(SocialMediaPlatform.Facebook, updatedRestaurant.SocialLinks.First().Platform);
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
                new() { Id = socialLinkId, Platform = SocialMediaPlatform.Twitter, Url = "twitter.com/test", RestaurantId = restaurantId }
            }
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        // Act
        var result = await _socialLinksService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
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
        var result = await _socialLinksService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Restaurant not found", result.Error!.Message);
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
            SocialLinks = new List<SocialLink>()
        };
    
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
    
        // Act
        var result = await _socialLinksService.DeleteSocialLinkAsync(restaurantId, socialLinkId);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Social link not found", result.Error!.Message);
    }
}
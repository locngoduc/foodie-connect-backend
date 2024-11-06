using System.Dynamic;
using System.Net;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;

namespace food_connect_backend.tests.UnitTests.Services;

public class RestaurantsServiceTests
{
    private readonly Mock<IUploaderService> _mockUploaderService;
    private readonly Mock<IGeoCoderService> _mockGeoCoderService;
    private readonly ApplicationDbContext _dbContext;
    private readonly RestaurantsService _service;

    public RestaurantsServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new ApplicationDbContext(options);

        _mockUploaderService = new Mock<IUploaderService>();
        _mockGeoCoderService = new Mock<IGeoCoderService>();

        _service = new RestaurantsService(
            _mockUploaderService.Object,
            _dbContext,
            _mockGeoCoderService.Object
        );
    }

    [Fact]
    public async Task CreateRestaurant_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var createDto = new CreateRestaurantDto
        {
            Name = "Test Restaurant",
            Phone = "+1234567890",
            OpenTime = 8,
            CloseTime = 22,
            LatitudeLongitude = "0,0",
            Status = RestaurantStatus.Open
        };

        var head = new User { Id = "user1" };

        _mockGeoCoderService.Setup(g => g.GetAddressAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(Result<Area>.Success(new Area
            {
                FormattedAddress = "123 Gourmet St, Foodie City, FL 12345, USA",
                StreetAddress = "123 Gourmet St",
                Route = "Gourmet St",
                Intersection = "Foodie Ave & Gourmet St",
                PoliticalEntity = "Foodie County",
                Country = "USA",
                AdministrativeAreaLevel1 = "Florida",
                AdministrativeAreaLevel2 = "Miami-Dade County",
                AdministrativeAreaLevel3 = "Downtown Miami",
                Locality = "Miami",
                Sublocality = "Downtown",
                Neighborhood = "Culinary District",
                PostalCode = "12345",
                PlusCode = "AB34+56",
                NaturalFeature = "Miami River",
                Airport = "Miami International Airport",
                Park = "Foodie Park",
                PointOfInterest = "Foodie Plaza",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Restaurants = new List<Restaurant>() 
            }));

        // Act
        var result = await _service.CreateRestaurant(createDto, head);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.Name, result.Value.Name);
        Assert.Equal(head.Id, result.Value.HeadId);
    }

    [Fact]
    public async Task GetRestaurantById_ExistingId_ReturnsRestaurant()
    {
        // Arrange
        var restaurant = new Restaurant
        {
            Id = "rest1",
            Name = "Test Restaurant",
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetRestaurantById("rest1");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(restaurant.Id, result.Value.Id);
        Assert.Equal(restaurant.Name, result.Value.Name);
    }

    [Fact]
    public async Task GetRestaurantById_NonExistentId_ReturnsFailure()
    {
        // Act
        var result = await _service.GetRestaurantById("nonexistent");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(RestaurantError.RestaurantNotExistCode, result.Error.Code);
    }

    [Fact]
    public async Task UploadLogo_ValidFile_ReturnsSuccess()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            Phone = "1234567890",
            HeadId = "user1",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.png");
        mockFile.Setup(f => f.Length).Returns(1024 * 1024); // 1MB
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));

        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Success($"foodie/restaurants/${restaurantId}/logo"));

        // Act
        var result = await _service.UploadLogo(restaurantId, mockFile.Object);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Once);
        
        var updatedRestaurant = await _dbContext.Restaurants.FindAsync(restaurantId);
        Assert.Contains($"foodie/restaurants/${restaurantId}/logo", updatedRestaurant!.Images);
    }

    [Fact]
    public async Task DeleteImage_ExistingImage_ReturnsSuccess()
    {
        // Arrange
        var restaurantId = "rest1";
        var imageId = "image1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string> { imageId },
            Phone = "1234567890",
            HeadId = "user1",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();
    
        _mockUploaderService.Setup(c => c.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Success(true));
    
        // Act
        var result = await _service.DeleteImage(restaurantId, imageId);
    
        // Assert
        Assert.True(result.IsSuccess);
        _mockUploaderService.Verify(c => c.DeleteFileAsync(It.IsAny<string>()), Times.Once);
        
        var updatedRestaurant = await _dbContext.Restaurants.FindAsync(restaurantId);
        Assert.DoesNotContain(imageId, updatedRestaurant!.Images);
    }
    
    [Fact]
    public async Task UploadImages_MultipleValidFiles_ReturnsSuccess()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();
    
        var mockFiles = new List<IFormFile>();
        for (int i = 0; i < 3; i++)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns($"test{i}.png");
            mockFile.Setup(f => f.Length).Returns(1024 * 1024);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));
            mockFiles.Add(mockFile.Object);
        }
    
        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .Returns((IFormFile file, ImageFileOptions options) => {
                var fileId = Path.GetFileNameWithoutExtension(file.FileName);
                return Task.FromResult(Result<string>.Success($"foodie/restaurants/{restaurantId}/{fileId}"));
            });
    
        // Act
        var result = await _service.UploadImages(restaurantId, mockFiles);
    
        // Assert
        Assert.True(result.IsSuccess);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Exactly(3));
        
        var updatedRestaurant = await _dbContext.Restaurants.FindAsync(restaurantId);
        Assert.Equal(3, updatedRestaurant!.Images.Count);
    }

    [Fact]
    public async Task UploadBanner_InvalidFileType_ReturnsFailure()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.gif"); // Invalid extension
        mockFile.Setup(f => f.Length).Returns(1024 * 1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));

        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(UploadError.TypeNotAllowed(".gif", [".jpg", ".png"])));

        // Act
        var result = await _service.UploadBanner(restaurantId, mockFile.Object);

        // Assert
        Assert.True(result.IsFailure);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Once);
    }
    
    [Fact]
    public async Task UploadBanner_ExceedFileSize_ReturnsFailure()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.png");
        mockFile.Setup(f => f.Length).Returns(1024 * 1024 * 6);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));

        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(UploadError.ExceedMaxSize(6*1024*1024, 5*1024*1024)));

        // Act
        var result = await _service.UploadBanner(restaurantId, mockFile.Object);

        // Assert
        Assert.True(result.IsFailure);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Once);
    }
    
    [Fact]
    public async Task UploadLogo_InvalidFileType_ReturnsFailure()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.gif"); // Invalid extension
        mockFile.Setup(f => f.Length).Returns(1024 * 1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));

        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(UploadError.TypeNotAllowed(".gif", [".jpg", ".png"])));

        // Act
        var result = await _service.UploadLogo(restaurantId, mockFile.Object);

        // Assert
        Assert.True(result.IsFailure);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Once);
    }
    
    [Fact]
    public async Task UploadLogo_ExceedFileSize_ReturnsFailure()
    {
        // Arrange
        var restaurantId = "rest1";
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Images = new List<string>(),
            HeadId = "user1",
            Phone = "1234567890",
            Location = new Point(new Coordinate(0,0))
        };
        await _dbContext.Restaurants.AddAsync(restaurant);
        await _dbContext.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.png");
        mockFile.Setup(f => f.Length).Returns(1024 * 1024 * 6);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test image")));

        _mockUploaderService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(UploadError.ExceedMaxSize(6*1024*1024, 5*1024*1024)));

        // Act
        var result = await _service.UploadLogo(restaurantId, mockFile.Object);

        // Assert
        Assert.True(result.IsFailure);
        _mockUploaderService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFileOptions>()), Times.Once);
    }
}
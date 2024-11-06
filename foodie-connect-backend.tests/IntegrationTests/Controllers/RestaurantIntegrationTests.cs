using System.Net;
using System.Net.Http.Headers;
using System.Text;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Shared.Dtos;
using Newtonsoft.Json;

namespace food_connect_backend.tests.IntegrationTests.Controllers;

public class RestaurantsIntegrationTests(FoodieWebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateRestaurant_ValidData_ReturnsCreatedRestaurant()
    {
        // Arrange
        var createDto = new CreateRestaurantDto
        {
            Name = "CreateRestaurant_ValidData",
            Address = "123 Test St",
            Phone = "1234567890"
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var restaurantOwner = await CreateAuthenticatedClientAsync("Head");
    
        // Act
        var response = await restaurantOwner.Client.PostAsync("/v1/restaurants", content);
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var restaurant = JsonConvert.DeserializeObject<Restaurant>(responseString);
        
        Assert.NotNull(restaurant);
        Assert.Equal(createDto.Name, restaurant.Name);
        Assert.Equal(restaurantOwner.User.Id, restaurant.HeadId);
        
        // Verify Location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/v1/restaurants/{restaurant.Id}", response.Headers.Location?.ToString());
    }
    
    [Fact]
    public async Task GetRestaurant_ExistingId_ReturnsRestaurant()
    {
        // Arrange - Create a restaurant first
        var createDto = new CreateRestaurantDto
        {
            Name = "GetRestaurant_ExistingId",
            Address = "123 Test St",
            Phone = "1234567890"
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");
        
        var restaurantOwner = await CreateAuthenticatedClientAsync("Head");
        var createResponse = await restaurantOwner.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);
    
        // Act
        var response = await restaurantOwner.Client.GetAsync($"/v1/restaurants/{createdRestaurant!.Id}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var restaurant = JsonConvert.DeserializeObject<Restaurant>(responseString);
        
        Assert.NotNull(restaurant);
        Assert.Equal(createdRestaurant.Id, restaurant.Id);
        Assert.Equal(createDto.Name, restaurant.Name);
        Assert.Equal(restaurantOwner.User.Id, createdRestaurant.HeadId);
    }
    
    [Fact]
    public async Task UpdateLogo_ValidFile_ReturnsSuccess()
    {
        // Arrange - Create a restaurant first
        var createDto = new CreateRestaurantDto
        {
            Name = "UpdateLogo_ValidFile",
            Address = "123 Test St",
            Phone = "1234567890"
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");
    
        var restaurantOwner = await CreateAuthenticatedClientAsync("Head");
        var createResponse = await restaurantOwner.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);
    
        // Create test image file
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
    
        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-logo.png");
    
        // Act
        var response = await restaurantOwner.Client.PutAsync($"/v1/restaurants/{createdRestaurant!.Id}/logo", formData);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        
        Assert.NotNull(result);
        Assert.Equal("Logo updated successfully", result.Message);
    }
    
    [Fact]
    public async Task UpdateLogo_UnauthorizedUser_ReturnsForbidden()
    {
        // Arrange - Create a restaurant with a different head ID
        var restaurantOwnerClient = await CreateAuthenticatedClientAsync("Head");
        var passerbyClient = await CreateAuthenticatedClientAsync("Head");
    
        var createDto = new CreateRestaurantDto
        {
            Name = "UpdateLogo_UnauthorizedUser",
            Address = "123 Test St",
            Phone = "1234567890"
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");
    
        var createResponse = await restaurantOwnerClient.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);
        
        // Create test image file
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
    
        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-logo.png");
    
        // Act
        var response = await passerbyClient.Client.PutAsync($"/v1/restaurants/{createdRestaurant!.Id}/logo", formData);
    
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBanner_ValidFile_ReturnsSuccess()
    {
        // Arrange - Create a restaurant first
        var createDto = new CreateRestaurantDto
        {
            Name = "UpdateBanner_ValidFile",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var restaurantOwner = await CreateAuthenticatedClientAsync("Head");
        var createResponse = await restaurantOwner.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);

        // Create test image file
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-banner.png");

        // Act
        var response = await restaurantOwner.Client.PutAsync($"/v1/restaurants/{createdRestaurant!.Id}/banner", formData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        
        Assert.NotNull(result);
        Assert.Equal("Logo updated successfully", result.Message);
    }

    [Fact]
    public async Task UpdateBanner_UnauthorizedUser_ReturnsForbidden()
    {
        // Arrange - Create a restaurant with a different head ID
        var restaurantOwnerClient = await CreateAuthenticatedClientAsync("Head");
        var passerbyClient = await CreateAuthenticatedClientAsync("Head");

        var createDto = new CreateRestaurantDto
        {
            Name = "UpdateBanner_UnauthorizedUser",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await restaurantOwnerClient.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);
        
        // Create test image file
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-banner.png");

        // Act
        var response = await passerbyClient.Client.PutAsync($"/v1/restaurants/{createdRestaurant!.Id}/banner", formData);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UploadImages_ValidFiles_ReturnsSuccess()
    {
        // Arrange - Create a restaurant first
        var createDto = new CreateRestaurantDto
        {
            Name = "UploadImages_ValidFiles",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var restaurantOwner = await CreateAuthenticatedClientAsync("Head");
        var createResponse = await restaurantOwner.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);

        // Create test image files
        var formData = new MultipartFormDataContent();
        
        var imageContent1 = new ByteArrayContent([0x42]);
        imageContent1.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        formData.Add(imageContent1, "files", "test-image1.png");
        
        var imageContent2 = new ByteArrayContent([0x43]);
        imageContent2.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        formData.Add(imageContent2, "files", "test-image2.png");

        // Act
        var response = await restaurantOwner.Client.PostAsync($"/v1/restaurants/{createdRestaurant!.Id}/images", formData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        
        Assert.NotNull(result);
        Assert.Equal("Images uploaded successfully", result.Message);
    }

    [Fact]
    public async Task UploadImages_UnauthorizedUser_ReturnsForbidden()
    {
        // Arrange - Create a restaurant with a different head ID
        var restaurantOwnerClient = await CreateAuthenticatedClientAsync("Head");
        var passerbyClient = await CreateAuthenticatedClientAsync("Head");

        var createDto = new CreateRestaurantDto
        {
            Name = "UploadImages_UnauthorizedUser",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await restaurantOwnerClient.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);

        // Create test image file
        var formData = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        formData.Add(imageContent, "files", "test-image.png");

        // Act
        var response = await passerbyClient.Client.PostAsync($"/v1/restaurants/{createdRestaurant!.Id}/images", formData);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteImage_UnauthorizedUser_ReturnsForbidden()
    {
        // Arrange - Create a restaurant with a different head ID
        var restaurantOwnerClient = await CreateAuthenticatedClientAsync("Head");
        var passerbyClient = await CreateAuthenticatedClientAsync("Head");

        var createDto = new CreateRestaurantDto
        {
            Name = "DeleteImage_UnauthorizedUser",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await restaurantOwnerClient.Client.PostAsync("/v1/restaurants", content);
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createdRestaurant = JsonConvert.DeserializeObject<Restaurant>(createResponseString);

        // Act
        var response = await passerbyClient.Client.DeleteAsync($"/v1/restaurants/{createdRestaurant!.Id}/images/test-image.png");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

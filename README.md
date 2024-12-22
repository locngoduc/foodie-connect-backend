# FoodSpot Management System - Backend Service 🚀

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple?style=flat&logo=.net)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-blue?style=flat&logo=.net)](https://docs.microsoft.com/en-us/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15.0-blue?style=flat&logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Latest-blue?style=flat&logo=docker)](https://www.docker.com/)
[![API Docs](https://img.shields.io/badge/API-Docs-green?style=flat&logo=swagger)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Backend service for the FoodSpot Management System, a comprehensive platform for restaurant discovery and management in Vietnam.

## 📑 Table of Contents
- [Related Repositories](#-related-repositories)
- [Features](#-features)
- [Technology Stack](#%EF%B8%8F-technology-stack)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Database Configuration](#-database-configuration)
- [API Documentation](#-api-documentation)
- [Deployment](#-deployment)
- [Contributing](#-contributing)
- [License](#-license)

## 📚 Related Repositories

- **Frontend Application**: [FoodSpot-Frontend](https://github.com/yourusername/foodspot-frontend)
- **Recommendation Service**: [FoodSpot-Recommendation](https://github.com/yourusername/foodspot-recommendation)

## 🌟 Features

### Core Features
- User authentication and authorization
- Restaurant and menu management
- Review and rating system
- Location-based services
- Promotion management
- Real-time recommendations
- Social media integration
- File upload handling
- Geolocation services

### Module-Specific Features
- **Dishes**: Menu management and categorization
- **Reviews**: Restaurant and dish review system
- **Promotions**: Special offers and event management
- **Users**: User management and verification
- **Restaurants**: Restaurant profile and settings
- **GeoCode**: Location-based services
- **Sessions**: User session management
- **Uploader**: Media file management

## 🛠️ Technology Stack

### Core Framework
- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **PostgreSQL 15.0** with PostGIS extension
- **AutoMapper** for object mapping
- **JWT** for authentication

### External Services
- **Cloudinary**: Media storage
- **MailTrap**: Email services
- **Google Maps API**: Geolocation services

### Development Tools
- **Docker & Docker Compose**
- **Swagger** for API documentation
- **Git** for version control

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 15.0+
- Docker (optional)
- IDE (Visual Studio 2022 or JetBrains Rider recommended)

### Local Development Setup

1. Clone the repository
```bash
git clone https://github.com/VaderNgo/foodie-connect-backend.git
cd foodie-connect-backend.api
```

2. Configure your environment
```bash
cp appsettings.Development.example.json appsettings.Development.json
# Update the configuration values in appsettings.Development.json
```

3. Install dependencies
```bash
dotnet restore
```

4. Set up the database
```bash
# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

5. Run the application
```bash
dotnet run
```

### Docker Setup

1. Build and run using Docker Compose
```bash
docker-compose up --build
#Or else run by the "Start" button from IDE you are using
```

## 🏗️ Project Structure

```
foodie-connect-backend.api/
├── Data/                      # Data access layer
├── Extensions/               # Extension methods
│   └── DI/                  # Dependency Injection configurations
├── Migrations/              # EF Core migrations
├── Modules/                 # Feature modules
│   ├── DishCategories/     
│   ├── DishReviews/        
│   ├── Dishes/             
│   ├── GeoCode/            
│   ├── Hotels/             
│   ├── Promotions/         
│   ├── Recommendations/    
│   ├── RestaurantReviews/  
│   ├── Restaurants/        
│   ├── Sessions/           
│   ├── Socials/           
│   ├── Uploader/          
│   ├── Users/             
│   └── Verification/      
├── Properties/            
├── Shared/               
├── Dockerfile            
├── Program.cs            
└── appsettings.json      
```

### Module Structure
Each feature module follows this structure:
```
Module/
├── Controller/           # API endpoints
├── Mapper/              # AutoMapper profiles
└── Service/             # Business logic implementation
```

## 💾 Database Configuration

### Connection String Format
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=foodspot;Username=your_username;Password=your_password"
  }
}
```

### Migration Commands
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📚 API Documentation

- Swagger UI is available at `/swagger` in development
- API endpoints are grouped by modules
- Authentication using JWT Bearer tokens
- Rate limiting applied to public endpoints

## 🚢 Deployment

### Production Setup
1. Update appsettings.json with production values
2. Set environment variables for sensitive data
3. Build the Docker image
```bash
docker build -t foodie-connect .
```

### Environment Variables
Required environment variables:
```
#View the .env.example
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
```bash
git checkout -b feature/YourFeature
```
3. Commit your changes
```bash
git commit -m 'Add some feature'
```
4. Push to the branch
```bash
git push origin feature/YourFeature
```
5. Open a Pull Request

### Coding Guidelines
- Follow C# coding conventions
- Use meaningful names for methods and variables
- Add XML comments for public APIs
- Include unit tests for new features
- Update documentation as needed

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.


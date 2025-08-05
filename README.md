# CarSellingPlatform 🚗

An ASP.NET Core MVC web application for buying and selling cars. The platform allows users to browse car listings, upload their own vehicles for sale, and communicate with sellers through a real-time chat system.

---
## 🎮 Test it 
-  [Google](carsellingapp-250805125548.azurewebsites.net)

---
## ✨ Features
- User Registration & Authentication
- List Cars for Sale with Images
- Browse, Search & Filter Car Listings
- Real-Time Chat Between Buyers and Sellers (SignalR)
- Responsive UI Design with Bootstrap
- Entity Framework Core for Data Access
- Modular Architecture with Clean Service Layer
- Integration & Unit Testing Support

---

## 🛠️ Tech Stack
- **Backend:** ASP.NET Core MVC (.NET 8)
- **Frontend:** HTML, CSS, Bootstrap 5, Razor Views
- **Real-Time Communication:** SignalR
- **Database:** Entity Framework Core (SQL Server)
- **Testing:** xUnit, Integration Tests
- **Architecture:** Layered Architecture (Web, Services, Data, ViewModels)

---

## 📂 Project Structure
CarSellingPlatform/
├── CarSellingPlatform.Web/ # ASP.NET Core Web App (Controllers, Views)
├── CarSellingPlatform.Data/ # Database Context and Repositories
├── CarSellingPlatform.Services.Core/ # Core Business Logic Services
├── CarSellingPlatform.Services.Common/ # Common Services
├── CarSellingPlatform.Web.ViewModels/ # View Models for Web Layer
├── CarSellingPlatform.Web.Infrastructure/ # Web Infrastructure Helpers (Middleware, etc.)
├── CarSellingPlatform.Data.Models/ # Entity Models
├── CarSellingPlatform.Services.AutoMapping/ # AutoMapper Profiles
├── CarSellingPlatform.GCommon/ # Shared/Common Utilities
├── CarSellingPlatform.Web.Tests/ # Unit Tests for Web Layer
├── CarSellingPlatform.Services.Core.Tests/ # Unit Tests for Service Layer
├── CarSellingPlatform.IntegrationTests/ # Integration Tests
└── CarSellingPlatform.sln # Solution File

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or modify connection string for your DB)
- Visual Studio 2022 or Visual Studio Code

### Installation & Running Locally
```bash
# Clone the repository
git clone https://github.com/Filse01/CarSellingPlatform.git
cd CarSellingPlatform

# Restore dependencies
dotnet restore

# Apply EF Core Migrations
dotnet ef database update --project CarSellingPlatform.Data

# Run the Application
dotnet run --project CarSellingPlatform.Web

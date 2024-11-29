# Short Term Stay API

## 🌐 Demo Video & Azure Deployment & Source Code  

- [Demo Video Link](https://www.youtube.com/watch?v=vxq3_uN50zY)
- [Azure Web App Link](https://shorttermstayapi-ataekren-fmekgcafc4c0bbb2.westeurope-01.azurewebsites.net/swagger/index.html)
- [GitHub Repository Link](https://github.com/ataekren/ShortTermStayWebAPI)

## 🎯 Project Overview

ShortTermStayAPI is a backend service that facilitates short-term property rentals

## 📊 Data Model

![ER](https://github.com/ataekren/ShortTermStayWebAPI/blob/master/ER.png?raw=true)

## 🏗 Design

### User Roles

The system implements three distinct user roles:

1. **Guest**
   - Regular users who can browse and book listings
   - Can write reviews for their stays

2. **Host**
   - Users who can create property listings

3. **Admin**
   - Admins can report listings with ratings

### Security

- JWT-based authentication with configurable token expiration
- Role-based authorization


### Key Entities

- Users
- Listings
- Bookings
- Reviews

## 🚀 Setup & Installation

1. Clone the repository
2. Copy `appsettings.json.Template` to `appsettings.json`
3. Configure the following settings:
   - Database connection string
   - JWT settings (Key, Issuer, Audience)
4. Run Entity Framework migrations:
   ```bash
   dotnet ef database update
   ```
5. Start the application:
   ```bash
   dotnet run
   ```

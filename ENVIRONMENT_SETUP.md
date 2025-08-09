# Environment Variables Setup Guide

## Overview
This project has been configured to use environment variables for sensitive configuration data instead of hardcoded values. This improves security by keeping secrets out of source control.

## Quick Setup for Development

### Option 1: Windows Environment Variables (Recommended for Development)

1. Open **System Properties** → **Advanced** → **Environment Variables**
2. Add the following **User variables**:

```
EMAIL_FROM_ADDRESS = zaydabduxamidov2008@gmail.com
EMAIL_USERNAME = zaydabduxamidov2008@gmail.com
EMAIL_PASSWORD = cswb wpoc skou kqyh
MINIO_ENDPOINT = localhost:9000
MINIO_ACCESS_KEY = minioadmin
MINIO_SECRET_KEY = minioadmin
JWT_SECRET_KEY = A7fK9mN2pQ8vX3zR6wE1yU4tI0oP5sD8gH7jL9kM2nB6vC3xZ1qW4eR7tY0uI3oP
DATABASE_CONNECTION_STRING = Server=localhost;Port=5432;Database=real_life_rpg_test;User Id=postgres;Password=postgres;
```

3. **Restart Visual Studio** or your IDE after setting environment variables
4. Run your application - it should now read from environment variables

### Option 2: Using .env File (Alternative)

1. Copy `.env.example` to `.env` in the root directory:
   ```bash
   copy .env.example .env
   ```

2. Install the `DotNetEnv` package:
   ```bash
   dotnet add package DotNetEnv
   ```

3. Add this code to your `Program.cs` at the very beginning:
   ```csharp
   using DotNetEnv;
   
   // Load environment variables from .env file
   Env.Load();
   ```

### Option 3: ASP.NET Core User Secrets (Development Only)

1. Right-click your project → **Manage User Secrets**
2. Add the configuration:
   ```json
   {
     "EmailConfiguration:DefaultFromEmail": "zaydabduxamidov2008@gmail.com",
     "EmailConfiguration:Username": "zaydabduxamidov2008@gmail.com",
     "EmailConfiguration:Password": "cswb wpoc skou kqyh",
     "MinioSettings:Endpoint": "localhost:9000",
     "MinioSettings:AccessKey": "minioadmin",
     "MinioSettings:SecretKey": "minioadmin",
     "JwtOption:SecretKey": "A7fK9mN2pQ8vX3zR6wE1yU4tI0oP5sD8gH7jL9kM2nB6vC3xZ1qW4eR7tY0uI3oP",
     "ConnectionStrings:DefaultConnection": "Server=localhost;Port=5432;Database=real_life_rpg_test;User Id=postgres;Password=postgres;"
   }
   ```

## Production Deployment

### Azure App Service
1. Go to **Configuration** → **Application settings**
2. Add each environment variable as a new application setting

### Docker
Add environment variables to your Docker run command:
```bash
docker run -e EMAIL_FROM_ADDRESS="zaydabduxamidov2008@gmail.com" \
           -e EMAIL_PASSWORD="cswb wpoc skou kqyh" \
           -e JWT_SECRET_KEY="A7fK9mN2pQ8vX3zR6wE1yU4tI0oP5sD8gH7jL9kM2nB6vC3xZ1qW4eR7tY0uI3oP" \
           your-app-image
```

### Azure Key Vault (Recommended for Production)
1. Create an Azure Key Vault
2. Add secrets with these names:
   - `EMAIL-PASSWORD`
   - `JWT-SECRET-KEY`
   - `DATABASE-CONNECTION-STRING`
   - etc.
3. Configure your app to read from Key Vault

## Security Notes

⚠️ **Important Security Reminders:**

1. **Never commit `.env` files** to version control
2. **Use different credentials** for development, staging, and production
3. **For Gmail**: Use App Passwords instead of your regular password
4. **JWT Secret**: Generate a new secure key for production (minimum 64 characters)
5. **Database**: Use strong passwords and restrict access in production
6. **Rotate secrets regularly** in production environments

## Troubleshooting

### App Can't Find Environment Variables
- **Windows**: Restart your IDE after setting environment variables
- **Docker**: Ensure environment variables are passed correctly
- **Azure**: Check Application Settings in the Azure portal

### Configuration Not Loading
1. Check spelling of environment variable names (case-sensitive on Linux)
2. Verify the format matches exactly what's expected
3. Check logs for configuration errors

### Development vs Production
- `appsettings.Development.json` contains the actual values for local development
- `appsettings.json` now uses environment variable placeholders for production
- This ensures smooth development while maintaining production security

## Testing Your Setup

Run this command to verify environment variables are set:
```bash
echo $EMAIL_FROM_ADDRESS  # Linux/Mac
echo %EMAIL_FROM_ADDRESS% # Windows CMD
```

Your application should start without errors if all environment variables are properly configured.
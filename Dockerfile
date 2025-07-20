# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (adjusted for correct paths)
COPY ["src/TaskManagement/TaskManagement.API.csproj", "TaskManagement/"]
COPY ["src/TaskManagement.Application/TaskManagement.Application.csproj", "TaskManagement.Application/"]
COPY ["src/TaskManagement.Domain/TaskManagement.Domain.csproj", "TaskManagement.Domain/"]
COPY ["src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj", "TaskManagement.Infrastructure/"]
COPY ["src/TaskManagement.Persistence/TaskManagement.Persistence.csproj", "TaskManagement.Persistence/"]

# Restore
RUN dotnet restore "TaskManagement/TaskManagement.API.csproj"

# Copy everything
COPY . .

# Build and publish
RUN dotnet publish "TaskManagement/TaskManagement.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]

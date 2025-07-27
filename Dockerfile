# Use official .NET SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY API/*.csproj ./API/
COPY Shared/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the files
COPY . .

# Publish the app to /out directory
RUN dotnet publish API/API.csproj -c Release -o /app/out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose port 80
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "API.dll"]

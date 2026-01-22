# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Landing.csproj", "./"]
COPY ["../Manager.Contracts/Manager.Contracts.csproj", "../Manager.Contracts/"]
RUN dotnet restore "Landing.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src"
RUN dotnet build "Landing.csproj" -c Release -o /app/build
RUN dotnet publish "Landing.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Render usa porta 10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000
ENTRYPOINT ["dotnet", "Landing.dll"]

# Build aşaması: .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projeyi tamamen kopyala
COPY . .

# Restore
WORKDIR /src/BluesenseChat.WebAPI
RUN dotnet restore

# Publish
RUN dotnet publish "BluesenseChat.WebAPI.csproj" -c Release -o /app/publish

# Runtime aşaması: ASP.NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Port ve entrypoint
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "BluesenseChat.WebAPI.dll"]

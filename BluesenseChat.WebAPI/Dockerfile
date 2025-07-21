# 1️⃣ Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BluesenseChat.WebAPI/BluesenseChat.WebAPI.csproj", "BluesenseChat.WebAPI/"]
COPY ["BluesenseChat.Application/BluesenseChat.Application.csproj", "BluesenseChat.Application/"]
COPY ["BluesenseChat.Domain/BluesenseChat.Domain.csproj", "BluesenseChat.Domain/"]
COPY ["BluesenseChat.Infrastructure/BluesenseChat.Infrastructure.csproj", "BluesenseChat.Infrastructure/"]


RUN dotnet restore "BluesenseChat.WebAPI/BluesenseChat.WebAPI.csproj"

COPY . .
WORKDIR "/src/BluesenseChat.WebAPI"
RUN dotnet publish "BluesenseChat.WebAPI.csproj" -c Release -o /app/publish

# 2️⃣ Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "BluesenseChat.WebAPI.dll"]

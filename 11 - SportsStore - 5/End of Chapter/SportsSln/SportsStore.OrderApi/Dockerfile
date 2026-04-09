FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SportsStore.OrderApi/SportsStore.OrderApi.csproj", "SportsStore.OrderApi/"]
COPY ["SportsStore.Shared/SportsStore.Shared.csproj", "SportsStore.Shared/"]
RUN dotnet restore "SportsStore.OrderApi/SportsStore.OrderApi.csproj"
COPY . .
WORKDIR "/src/SportsStore.OrderApi"
RUN dotnet build "SportsStore.OrderApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportsStore.OrderApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportsStore.OrderApi.dll"]

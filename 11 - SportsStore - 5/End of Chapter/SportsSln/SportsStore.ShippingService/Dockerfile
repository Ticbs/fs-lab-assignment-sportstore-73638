FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SportsStore.ShippingService/SportsStore.ShippingService.csproj", "SportsStore.ShippingService/"]
COPY ["SportsStore.Shared/SportsStore.Shared.csproj", "SportsStore.Shared/"]
RUN dotnet restore "SportsStore.ShippingService/SportsStore.ShippingService.csproj"
COPY . .
WORKDIR "/src/SportsStore.ShippingService"
RUN dotnet build "SportsStore.ShippingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportsStore.ShippingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportsStore.ShippingService.dll"]

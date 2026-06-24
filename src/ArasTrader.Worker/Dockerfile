FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

WORKDIR /src

COPY . .

RUN dotnet restore ArasTrader.Worker/ArasTrader.Worker.csproj

RUN dotnet publish ArasTrader.Worker/ArasTrader.Worker.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ArasTrader.Worker.dll"]
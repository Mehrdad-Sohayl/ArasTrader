FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

ENV DOTNET_SYSTEM_NET_SECURITY_SSLREVOCATIONCHECKMODE=NoCheck

WORKDIR /src

COPY . .

RUN dotnet restore ArasTrader.Api/ArasTrader.Api.csproj

RUN dotnet publish ArasTrader.Api/ArasTrader.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ArasTrader.Api.dll"]
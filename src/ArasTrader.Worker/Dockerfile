FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

ENV DOTNET_SYSTEM_NET_SECURITY_SSLREVOCATIONCHECKMODE=NoCheck
ENV DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT=0
ENV NUGET_XMLDOC_MODE=skip

WORKDIR /src

COPY . .

RUN dotnet restore ArasTrader.Worker/ArasTrader.Worker.csproj --disable-parallel

RUN dotnet publish ArasTrader.Worker/ArasTrader.Worker.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ArasTrader.Worker.dll"]
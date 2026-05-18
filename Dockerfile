FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore MsAtracciones.Api/MsAtracciones.Api.csproj
RUN dotnet publish MsAtracciones.Api/MsAtracciones.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["dotnet", "MsAtracciones.Api.dll"]
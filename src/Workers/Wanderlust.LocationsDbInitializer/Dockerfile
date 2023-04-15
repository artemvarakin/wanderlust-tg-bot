FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

COPY /Libraries /Libraries
WORKDIR /workers/app

COPY Workers/Wanderlust.LocationsDbInitializer .
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime

WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Wanderlust.LocationsDbInitializer.dll"]
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

COPY ./Libraries/Wanderlust.Core ./Libraries/Wanderlust.Core/
COPY ./Libraries/Wanderlust.TravelpayoutsClients ./Libraries/Wanderlust.TravelpayoutsClients/
COPY ./Workers/Wanderlust.LocationsDbInitializer ./Workers/Wanderlust.LocationsDbInitializer/

WORKDIR /src/Workers/Wanderlust.LocationsDbInitializer
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime

WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Wanderlust.LocationsDbInitializer.dll"]
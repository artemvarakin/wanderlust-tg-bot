FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

COPY ./Libraries/Wanderlust.Core ./Libraries/Wanderlust.Core/
COPY ./Wanderlust.Application ./Wanderlust.Application/
COPY ./Wanderlust.Infrastructure ./Wanderlust.Infrastructure/
COPY ./Wanderlust.Web ./Wanderlust.Web/

WORKDIR /src/Wanderlust.Web
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime

WORKDIR /app

COPY --from=build /out .

ENTRYPOINT [ "dotnet", "Wanderlust.Web.dll"]

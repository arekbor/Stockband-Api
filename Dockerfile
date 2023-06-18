FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

LABEL maintainer="stockband-api"
RUN groupadd -r -g 2500 stockband-api
RUN useradd -r -g 2500 -u 3200 stockband-api

RUN chsh -s /usr/sbin/nologin root

USER stockband-api

WORKDIR "/app"
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR "/src"
COPY --chown=3200:2500 . .
WORKDIR "/src/."

RUN dotnet restore

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish --chown=3200:2500  /app/publish .

ENTRYPOINT ["dotnet", "Stockband.Api.dll"]


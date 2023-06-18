FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY . .
WORKDIR "/src/."
RUN dotnet restore

#RUN dotnet build "./Stockband.Api/Stockband.Api.csproj" -c Release -o /app/build

#FROM build AS publish
#RUN dotnet publish "./Stockband.Api/Stockband.Api.csproj" -c Release -o /app/publish

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .

#ENTRYPOINT ["dotnet", "StreamerApi.dll"]


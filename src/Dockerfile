FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
COPY ./Amphibian.Patrol.Training.Api ./Amphibian.Patrol.Training.Api
COPY ./Amphibian.Patrol.Training.Configuration ./Amphibian.Patrol.Training.Configuration
RUN dotnet publish Amphibian.Patrol.Training.Api/Amphibian.Patrol.Training.Api.csproj -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["/app/Amphibian.Patrol.Training.Api"]
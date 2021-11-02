FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as api-build
WORKDIR /app
COPY . .
RUN dotnet publish -r linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY  --from=api-build  /app/Amphibian.Patrol.Api/bin/Debug/net5.0/linux-x64/publish/ .
ENTRYPOINT ["/app/Amphibian.Patrol.Api"]
LABEL org.opencontainers.image.source https://github.com/brookpatten/patrol.ski
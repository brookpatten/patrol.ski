FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as api-build
WORKDIR /app
COPY . .
RUN dotnet publish -r linux-x64

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY  --from=api-build  /app/Amphibian.Patrol.Api/bin/Debug/netcoreapp3.1/linux-x64/publish/ .
ENTRYPOINT ["/app/Amphibian.Patrol.Api"]
LABEL org.opencontainers.image.source https://github.com/brookpatten/patrol.ski
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as api-build
WORKDIR /app
COPY . .
RUN dotnet publish -r linux-x64

FROM node AS frontend-build
WORKDIR /source
COPY Amphibian.Patrol.Web .
RUN npm install
RUN npm rebuild node-sass
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY  --from=api-build  /app/Amphibian.Patrol.Api/bin/Debug/net5.0/linux-x64/publish/ .
COPY  --from=frontend-build  /source/dist ./static
ENTRYPOINT ["/app/Amphibian.Patrol.Api"]
LABEL org.opencontainers.image.source https://github.com/brookpatten/patrol.ski
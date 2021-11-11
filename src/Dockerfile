FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as api-build
ARG VERSION=local-build
WORKDIR /app
COPY . .
RUN echo -n "{\"Depot\":\"" > ./Amphibian.Patrol.Api/appsettings.version.json \
    && echo -n $VERSION >> ./Amphibian.Patrol.Api/appsettings.version.json \
	&& echo -n "\"}}" >> ./Amphibian.Patrol.Api/appsettings.version.json
RUN dotnet publish -r linux-x64 -p:Version=${VERSION:1}.0 -p:AssemblyVersion=${VERSION:1}.0

FROM node AS frontend-build
WORKDIR /source
COPY Amphibian.Patrol.Web .
RUN npm install
RUN npm rebuild node-sass
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY  --from=api-build  /app/Amphibian.Patrol.Api/bin/Debug/net6.0/linux-x64/publish/ .
COPY  --from=frontend-build  /source/dist ./static
ENTRYPOINT ["/app/Amphibian.Patrol.Api"]
LABEL org.opencontainers.image.source https://github.com/brookpatten/patrol.ski
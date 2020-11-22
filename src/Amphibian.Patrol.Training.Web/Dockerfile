FROM node AS build
WORKDIR /source
COPY . .
RUN npm install
RUN npm rebuild node-sass
RUN npm run build

# final stage/image
FROM nginx
WORKDIR /app
COPY --from=build /source/dist /usr/share/nginx/html
LABEL org.opencontainers.image.source https://github.com/brookpatten/patrol.training
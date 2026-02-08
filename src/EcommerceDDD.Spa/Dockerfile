FROM node:22-alpine AS build
WORKDIR /app
COPY src/EcommerceDDD.Spa/package*.json ./
RUN npm ci
COPY src/EcommerceDDD.Spa/ ./
RUN npm run build

FROM nginx:alpine
COPY src/EcommerceDDD.Spa/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/ecommerceddd-spa/browser /usr/share/nginx/html
EXPOSE 80

FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci --ignore-scripts
COPY . .
ENV NODE_OPTIONS=--openssl-legacy-provider
RUN npm run generate
FROM nginxinc/nginx-unprivileged:1.28-alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080

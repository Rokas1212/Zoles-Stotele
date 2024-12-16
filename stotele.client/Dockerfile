# Stage 1: Build the frontend
FROM node:19 AS build

# Install .NET SDK
RUN apt-get update && apt-get install -y wget \
    && wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0

WORKDIR /app

# Create directory for HTTPS certificate
RUN mkdir -p /root/.aspnet/https

COPY stotele.client/package*.json ./
RUN npm install

COPY stotele.client/. .
RUN npm run build

# Stage 2: Serve the frontend
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 5173
CMD ["nginx", "-g", "daemon off;"]
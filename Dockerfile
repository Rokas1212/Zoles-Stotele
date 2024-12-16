# Stage 1: Build and publish the .NET 8 backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /app
COPY Stotele.Server/*.csproj ./Stotele.Server/
WORKDIR /app/Stotele.Server
RUN dotnet restore
COPY Stotele.Server/. .
RUN dotnet publish -c Release -o out

# Stage 2: Build the React frontend
FROM node:18 AS frontend-builder
WORKDIR /app
COPY stotele.client/ ./
# Even though we're setting NODE_ENV=production, we still need devDependencies for the build
ENV NODE_ENV=production
RUN npm install --include=dev
RUN npm run build


# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
COPY --from=backend-builder /app/Stotele.Server/out ./
COPY --from=frontend-builder /app/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Stotele.Server.dll"]

# Use the official .NET image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5210

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Stotele.Server/Stotele.Server.csproj", "Stotele.Server/"]
RUN dotnet restore "Stotele.Server/Stotele.Server.csproj"
COPY . .
WORKDIR "/src/Stotele.Server"
RUN dotnet build "Stotele.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stotele.Server.csproj" -c Release -o /app/publish --no-dependencies

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stotele.Server.dll"]
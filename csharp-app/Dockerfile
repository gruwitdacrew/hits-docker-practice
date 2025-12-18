FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5146

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Application/Mockups/Mockups.csproj", "Application/Mockups/"]
RUN dotnet restore "Application/Mockups/Mockups.csproj"
COPY . .
WORKDIR "/src/Application/Mockups"
RUN dotnet build "Mockups.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mockups.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Устанавливаем переменную окружения для настройки URL
ENV ASPNETCORE_URLS=http://+:5146

ENTRYPOINT ["dotnet", "Mockups.dll"]
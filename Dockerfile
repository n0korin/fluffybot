# Сборка
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем пакеты
COPY *.csproj ./
RUN dotnet restore

# Копируем весь код
COPY . ./

# Публикуем релиз
RUN dotnet publish -c Release -o out

# Runtime-образ
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out ./

# Точка входа
ENTRYPOINT ["dotnet", "TelegramBotDemo.dll"]


# Используем официальный .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Копируем проект и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальные файлы и билдим
COPY . ./
RUN dotnet publish -c Release -o out

# Используем runtime-образ для запуска
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/out ./

# Устанавливаем переменную окружения (необязательно)
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Команда запуска
CMD ["dotnet", "TelegramBotDemo.dll"]

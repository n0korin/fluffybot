@echo off
REM --- Путь к dotnet ---
set DOTNET_PATH="C:\Program Files\dotnet\dotnet.exe"

REM --- Путь к проекту бота ---
set BOT_PATH="C:\Users\catbo\fluffybot\TelegramBotDemo\TelegramBotDemo.csproj"

REM --- Токен бота ---
set BOT_TOKEN=-----

REM --- Переходим в папку проекта ---
cd /d "C:\Users\catbo\fluffybot\TelegramBotDemo"

REM --- Запуск бота ---
echo Запуск Telegram бота...
%DOTNET_PATH% run --project %BOT_PATH%
pause

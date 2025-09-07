using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using Microsoft.AspNetCore.Builder; // для WebApplication

class Program
{
    private static readonly string Token =
        Environment.GetEnvironmentVariable("BOT_TOKEN")
        ?? throw new Exception("BOT_TOKEN не задан!");

    private static readonly string SiteUrl = "https://natribu.org";
    private static readonly string ManagerUrl = "https://t.me/Fluffy_Manager";
    private static readonly string ReviewsUrl = "https://docs.google.com/";

    static async Task Main()
    {
        // Запускаем Telegram-бота
        var bot = new TelegramBotClient(Token);
        bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandleErrorAsync
        );

        Console.WriteLine("Бот запущен...");

        // --- Минимальный Web Service для Render ---
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Один эндпоинт, чтобы Render видел порт как активный
        app.MapGet("/", () => "Bot is running!");

        // Привязываем к порту Render
        app.Urls.Add($"http://*:{port}");

        // Запускаем Web Service в фоне
        _ = Task.Run(() => app.Run());

        // Держим главный поток живым
        await Task.Delay(-1);
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message) return;
        var text = message.Text ?? string.Empty;

        if (text == "/start")
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("🌐 Сайт"), new KeyboardButton("💬 Менеджер") },
                new[] { new KeyboardButton("⭐ Отзывы") }
            })
            { ResizeKeyboard = true };

            await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Привет 👋 Добро пожаловать!\n\nВыберите действие на клавиатуре снизу 👇",
                replyMarkup: keyboard,
                cancellationToken: ct
            );
            return;
        }

        if (text == "🌐 Сайт")
            await bot.SendTextMessageAsync(message.Chat.Id, $"Наш сайт: {SiteUrl}", cancellationToken: ct);
        else if (text == "💬 Менеджер")
            await bot.SendTextMessageAsync(message.Chat.Id, $"Связаться с менеджером: {ManagerUrl}", cancellationToken: ct);
        else if (text == "⭐ Отзывы")
            await bot.SendTextMessageAsync(message.Chat.Id, $"Отзывы: {ReviewsUrl}", cancellationToken: ct);
        else
            await bot.SendTextMessageAsync(message.Chat.Id, "Выберите кнопку снизу 👇", cancellationToken: ct);
    }

    private static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Task.CompletedTask;
    }
}

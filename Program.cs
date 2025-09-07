using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static readonly string Token = Environment.GetEnvironmentVariable("7629993792:AAF92f-nvxQB-TaCdnJ7FS9BnlsZvntlF7s");
    private static readonly string SiteUrl = "https://natribu.org";
    private static readonly string ManagerUrl = "https://t.me/Fluffy_Manager";
    private static readonly string ReviewsUrl = "https://docs.google.com/";

    static async Task Main()
    {
        var botClient = new TelegramBotClient(Token);
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Бот @{me.Username} запущен...");

        using var cts = new CancellationTokenSource();

        // простой цикл получения обновлений
        var offset = 0;
        while (!cts.Token.IsCancellationRequested)
        {
            var updates = await botClient.GetUpdatesAsync(offset, cancellationToken: cts.Token);
            foreach (var update in updates)
            {
                offset = update.Id + 1;
                await HandleUpdateAsync(botClient, update, cts.Token);
            }

            await Task.Delay(1000);
        }
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is null || update.Message.Text is null) return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text;

        if (text == "/start")
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("🌐 Сайт"), new KeyboardButton("💬 Менеджер") },
                new[] { new KeyboardButton("⭐ Отзывы") }
            })
            { ResizeKeyboard = true };

            await botClient.SendTextMessageAsync(chatId, "Привет 👋 Добро пожаловать!\nВыберите действие на клавиатуре снизу 👇", replyMarkup: keyboard);
            return;
        }

        if (text == "🌐 Сайт")
            await botClient.SendTextMessageAsync(chatId, $"Наш сайт: {SiteUrl}");
        else if (text == "💬 Менеджер")
            await botClient.SendTextMessageAsync(chatId, $"Связаться с менеджером: {ManagerUrl}");
        else if (text == "⭐ Отзывы")
            await botClient.SendTextMessageAsync(chatId, $"Отзывы: {ReviewsUrl}");
        else
            await botClient.SendTextMessageAsync(chatId, "Выберите кнопку снизу 👇");
    }
}

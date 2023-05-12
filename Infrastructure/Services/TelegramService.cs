using Application.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Infrastructure.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly string _telegramBotToken;

        public TelegramService(string telegramBotToken)
        {
            _telegramBotToken = telegramBotToken;
        }

        public async Task SendMessageAsync(string chatId, string message)
        {
            var botClient = new TelegramBotClient(_telegramBotToken);
                await botClient.SendTextMessageAsync(chatId, message);
        }
    }
}

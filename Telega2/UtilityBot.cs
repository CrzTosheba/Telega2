using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UtilityBot
{
    class BotText : BackgroundService
    {
        /// <summary>
        /// объект, отвеающий за отправку сообщений клиенту
        /// </summary>
        private ITelegramBotClient _telegramClient;

        public BotText(ITelegramBotClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, // receive all update types
                cancellationToken: stoppingToken);

            Console.WriteLine("Bot started");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _telegramClient.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Данный тип сообщений не поддерживается. Пожалуйста отправьте текст.", cancellationToken: cancellationToken);
                return;
            }

            // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                switch (update.Message!.Type)
                {
                    case MessageType.Text:
                        await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Длина сообщения: {update.Message.Text.Length} знаков", cancellationToken: cancellationToken);
                        return;
                    default: // unsupported message
                        await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Данный тип сообщений не поддерживается. Пожалуйста отправьте текст.", cancellationToken: cancellationToken);
                        return;
                }
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            Console.WriteLine("Waiting 10 seconds before retry");
            Thread.Sleep(10000);
            return Task.CompletedTask;
        }
    }
}
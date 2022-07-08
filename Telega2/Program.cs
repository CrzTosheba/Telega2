using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telega2.Controllers;
using Telegram.Bot;
using Telega2;
using UtilityBot;

namespace Telega2
{
    static class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services))
                .UseConsoleLifetime()// Позволяет поддерживать приложение активным в консоли
                .Build();

            Console.WriteLine("Стартуем!!!");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            // Подключаем контроллеры сообщений и кнопок
       
            services.AddTransient<TextMessageController>();
          
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("5344280901:AAGNsq45lXc7u1SZdzDVrLHid8yVVA2u5TU"));
            //services.AddHostedService<Bot>();
            services.AddHostedService<BotText>();
        }
    }
}
using Pastel;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net.NetworkInformation;
namespace TelegramBotExperiments
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5464243937:AAGvwcbuW5Yqre9D-2_XKr2_Yw3YDwP1qb8");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Chat.Type == ChatType.Private)
                {
                    Console.WriteLine($"Чат:\n \t{"Firstname:".Pastel("#43c924")} {message.Chat.FirstName}\n \t{"LastName:".Pastel("#43c924")} {message.Chat.LastName}\n \t{"Title:".Pastel("#43c924")} {message.Chat.Title}\n \t{"Id:".Pastel("#43c924")} {message.Chat.Id}\n \t{"Text:".Pastel("#43c924")} {message.Text}");
                    if (message.Text.ToLower() == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Дарова, если тебе нужно расписание на сегодня для твоей группы в ХНУРЭ - я его сгенерирую для тебя. \n \n" +
                                                                           "Но учти, что ты должен быть одним из \"Избранных\", подробнее в ЛС у автора (@ketronix_dev)");
                        return;
                    }
                }

                if (message.Text == "/shedule" || message.Text == "/shedule@" + bot.GetMeAsync().Result.Username)
                {
                    Console.WriteLine($"Чат:\n \t{"Firstname:".Pastel("#43c924")} {message.From.FirstName}\n \t{"LastName:".Pastel("#43c924")} {message.From.LastName}\n \t{"Title:".Pastel("#43c924")} {message.Chat.Title}\n \t{"Id:".Pastel("#43c924")} {message.Chat.Id}\n \t{"Text:".Pastel("#43c924")} {message.Text}");
                    if (message.Chat.Id == -1001840161407)
                    {
                        var ping = new Ping();
                        var source = new Uri("https://cist.nure.ua/");
                        var isAlive = ping.SendPingAsync(source.Host, 500);

                        if (isAlive.Result.Status == IPStatus.Success)
                        {
                            await Service.GenerateHtmLforShedule(
                                $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}",
                                $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}");
                            var shedule = Service.GetCistShedule("simple.html");
                            var msg = await botClient.SendTextMessageAsync(message.Chat, shedule, ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else
                        {
                            var msg = await botClient.SendTextMessageAsync(message.Chat,
                                "Походу сервак с расписанием лег...");
                        }
                    }
                    else if(message.Chat.Id == -1001576440434)
                    {
                        /*var msg = await botClient.SendTextMessageAsync(message.Chat, "Расписание можно генерировать только если вы не пидоры, а вы кто? Правильно. Пидоры. Нехуй было банить.");
                    */
                    }
                    else
                    {
                        var msg = await botClient.SendTextMessageAsync(message.Chat,
                            "Расписание можно генерировать только если вы находитесь в разрешенной группе");
                    }
                }
                
                if (message.Text == "/shedule_week" || message.Text == "/shedule_week@" + bot.GetMeAsync().Result.Username)
                {
                    Console.WriteLine($"------------------------\n \t{"Firstname:".Pastel("#43c924")} {message.From.FirstName}\n \t{"LastName:".Pastel("#43c924")} {message.From.LastName}\n \t{"Title:".Pastel("#43c924")} {message.Chat.Title}\n \t{"Id:".Pastel("#43c924")} {message.Chat.Id}\n \t{"Text:".Pastel("#43c924")} {message.Text}");
                    if (message.Chat.Id == -1001840161407)
                    {
                        var ping = new Ping();
                        var source = new Uri("https://cist.nure.ua/");
                        var isAlive = ping.SendPingAsync(source.Host, 500);

                        if (isAlive.Result.Status == IPStatus.Success)
                        {
                            await Service.GenerateHtmLforShedule(
                                $"{Service.GetWeek()[0].Day}.{Service.GetWeek()[0].Month}.{Service.GetWeek()[0].Year}",
                                $"{Service.GetWeek()[5].Day}.{Service.GetWeek()[5].Month}.{Service.GetWeek()[5].Year}");

                            var msg = await botClient.SendTextMessageAsync(message.Chat, Service.GetWeekShedule(),
                                ParseMode.Html, disableWebPagePreview: true);
                        }
                        else
                        {
                            var msg = await botClient.SendTextMessageAsync(message.Chat,
                                "Походу сервак с расписанием лег...");
                        }
                    }
                    else if(message.Chat.Id == -1001576440434)
                    {
                        /*var msg = await botClient.SendTextMessageAsync(message.Chat, "Расписание можно генерировать только если вы не пидоры, а вы кто? Правильно. Пидоры. Нехуй было банить.");
                    */
                    }
                    else
                    {
                        var msg = await botClient.SendTextMessageAsync(message.Chat,
                            "Расписание можно генерировать только если вы находитесь в разрешенной группе");
                    }
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(exception);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
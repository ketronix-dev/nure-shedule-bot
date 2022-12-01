using Pastel;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net.NetworkInformation;
using Bot.DateManagment;
using Bot.Logging;
using File = System.IO.File;
using System.Text;
using Tomlyn;

namespace Bot
{

    class Program
    {
        private static ITelegramBotClient bot;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                var week = Date.GetWeek();
                var day = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
                var dayOld = $"{DateTime.Today.AddDays(-1).Day}.{DateTime.Today.AddDays(-1).Month}.{DateTime.Today.AddDays(-1).Year}";

                try
                {
                    if (!File.Exists($"./day-{day}.html") && !File.Exists($"./week-{week[4]}.html"))
                    {
                        await Service.GenerateHtmLforShedule(day, day, false);
                        await Service.GenerateHtmLforShedule($"{week[0]}", $"{week[4]}", true);
                        await botClient.SendTextMessageAsync(946530105, "Расписание загружено в кеш.");
                    }
                
                    if (File.Exists($"./day-{dayOld}.html") && File.Exists($"./week-{Date.GetLastDayOnPreviousWeek()}.html"))
                    {
                        File.Delete($"./day-{dayOld}.html");
                        File.Delete($"./week-{Date.GetLastDayOnPreviousWeek()}.html");
                        await botClient.SendTextMessageAsync(946530105, "Устаревшое расписание было удалено.");
                    }
                    
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(946530105,
                        $"При попытке обновить кеш произошла ошибка: {e.Message}");
                }

                if (message.Chat.Type == ChatType.Private)
                {
                    await botClient.SendTextMessageAsync(946530105, Log.LogMessage(
                        message.From.FirstName,
                        message.From.LastName,
                        message.Chat.Title,
                        message.Chat.Id,
                        message.Text));
                    if (message.Text.ToLower() == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Дарова, если тебе нужно расписание на сегодня для твоей группы в ХНУРЭ - я его сгенерирую для тебя. \n \n" +
                                                                           "Но учти, что ты должен быть одним из \"Избранных\", подробнее в ЛС у автора (@ketronix_dev)");
                        return;
                    }

                    if (message.Text.ToLower().Contains("/notify_update") && message.From.Id == 946530105)
                    {
                        var textToSend = message.Text.Split('|')[1];

                        try
                        {
                            await botClient.SendTextMessageAsync(-1001840161407,
                                textToSend);
                            await botClient.SendTextMessageAsync(-1001576440434,
                                textToSend);
                            
                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                $"Пользователи только что были уведомлены о обновлении, его текст: \n \n {textToSend}");
                        }
                        catch (Exception e)
                        {
                            await botClient.SendTextMessageAsync(946530105,
                                $"При попытке отправить уведомление произошла ошибка: {e.Message}");
                        }
                    }
                }

                if (message.Text == "/shedule" || message.Text == "/shedule@" + bot.GetMeAsync().Result.Username)
                {
                    await botClient.SendTextMessageAsync(946530105, Log.LogMessage(
                        message.From.FirstName,
                        message.From.LastName,
                        message.Chat.Title,
                        message.Chat.Id,
                        message.Text));
                    
                    if (message.Chat.Id == -1001840161407 || message.Chat.Id == -1001576440434)
                    {
                        var ping = new Ping();
                        var source = new Uri("https://cist.nure.ua/");
                        var isAlive = ping.SendPingAsync(source.Host, 500);

                        if (isAlive.Result.Status == IPStatus.Success)
                        {
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
                    else
                    {
                        var msg = await botClient.SendTextMessageAsync(message.Chat,
                            "Расписание можно генерировать только если вы находитесь в разрешенной группе");
                    }
                }
                
                if (message.Text == "/shedule_week" || message.Text == "/shedule_week@" + bot.GetMeAsync().Result.Username)
                {
                    await botClient.SendTextMessageAsync(946530105, Log.LogMessage(
                        message.From.FirstName,
                        message.From.LastName,
                        message.Chat.Title,
                        message.Chat.Id,
                        message.Text));
                    if (message.Chat.Id == -1001840161407 || message.Chat.Id == -1001576440434)
                    {
                        var ping = new Ping();
                        var source = new Uri("https://cist.nure.ua/");
                        var isAlive = ping.SendPingAsync(source.Host, 500);

                        if (isAlive.Result.Status == IPStatus.Success)
                        {
                            var msg = await botClient.SendTextMessageAsync(message.Chat,
                                Service.GetWeekShedule($"./week-{week[4]}.html"),
                                ParseMode.Html, disableWebPagePreview: true);
                        }
                        else
                        {
                            var msg = await botClient.SendTextMessageAsync(message.Chat,
                                "Походу сервак с расписанием лег...");
                        }
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
            if (!File.Exists("config.toml"))
            {
                Console.Write("Please enter the token from the bot: ");
                string tokenBot = Console.ReadLine();

                using (FileStream fstream = new FileStream("config.toml", FileMode.OpenOrCreate))
                {
                    string configText =
                            String.Format("botToken = '{0}'",
                        tokenBot);
                    Console.WriteLine(configText);
                    byte[] buffer = Encoding.Default.GetBytes(configText);
                    fstream.Write(buffer, 0, buffer.Length);
                }
            }
            using (FileStream fstream = File.OpenRead("config.toml"))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);

                var model = Toml.ToModel(textFromFile);
                bot = new TelegramBotClient((string) model["botToken"]!);;
            }

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
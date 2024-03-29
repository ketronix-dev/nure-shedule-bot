﻿using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using File = System.IO.File;
using System.Text;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using Tomlyn;
using NureBotSchedule.DatabaseUtils;
using NureBotSchedule.Services;
using NureBotSchedule.Services.ScheduleServices;
using NureBotSchedule.DateManagment;
using NureBotSchedule.Generators;


namespace NureBotSchedule
{
    class Program
    {
        private static string Key;
        private static Message join;

        static ITelegramBotClient bot = new TelegramBotClient("");

        static async Task ExecuteCode(ITelegramBotClient botClient, Update update)
        {
            try
            {
                await botClient.SendTextMessageAsync(
                    -1001638301850,
                    Newtonsoft.Json.JsonConvert.SerializeObject(update, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Code execution failed: " + ex.ToString());
            }

            Thread.Sleep(3000);
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                DbUtils.CreateTableOrNo();
                if (update.Type == UpdateType.Message)
                {
                    /*Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update, Formatting.Indented));*/
                    var message = update.Message;

                    if (message is not null && message.Type == MessageType.Text)
                    {
                        if (message.Text.Contains("/schedule") && DbUtils.checkBlockGroup(message.Chat.Id))
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Ця група була заблокована адміністратором бота, за подробицями пишіть адміністратору бота." +
                                "\n \n Будь-ласка, не намагайтеся написати повідомлення для адміністратора в приватні повідомлення боту, адміністратор все одно їх так не побачить.");
                        }
                        else
                        {
                            if (message.Chat.Type == ChatType.Private)
                            {
                                if (DbUtils.checkGroup(message.Chat.Id))
                                {
                                    if (message.Text.ToLower().Contains("/ban") && message.From.Id == 946530105)
                                    {
                                        var toBan = long.Parse(message.Text.Split('|')[1]);

                                        try
                                        {
                                            DbUtils.InsertGroupToBlock(toBan);

                                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                                $"Ви тільки но заблокували групу: \n \n {toBan}");
                                        }
                                        catch (Exception e)
                                        {
                                            await botClient.SendTextMessageAsync(946530105,
                                                $"Група не була заблокована: {e.Message}");
                                        }
                                    }

                                    if (message.Text == "/schedule" ||
                                        message.Text == "/schedule@" + botClient.GetMeAsync().Result.Username)
                                    {
                                        var events = Schedule.GetCistEvents(
                                            Schedule.GetCistShedule(DbUtils.GetGroup(message.Chat.Id), Key),
                                            DateService.GetToday(),
                                            DateService.GetToday());
                                        var messageSchedule = MessageGenerator.GenerateMessageForToday(
                                            events,
                                            DbUtils.GetGroup(message.Chat.Id));

                                        if (events.Count is not 0)
                                        {
                                            await botClient.SendTextMessageAsync(
                                                message.Chat.Id,
                                                messageSchedule,
                                                parseMode: ParseMode.Html, disableWebPagePreview: true);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(
                                                message.Chat.Id,
                                                "Пар сьогодні нема, дозволяю відпочити");
                                        }
                                    }

                                    if (message.Text == "/schedule_week" ||
                                        message.Text == "/schedule_week@" + botClient.GetMeAsync().Result.Username)
                                    {
                                        var weekDates = DateService.GetWeekDates(DateService.GetToday());
                                        var messageSchedule = MessageGenerator.GenerateMessageForWeek(
                                            DbUtils.GetGroup(message.Chat.Id), weekDates[0], weekDates[1], Key);

                                        await botClient.SendTextMessageAsync(
                                            message.Chat.Id,
                                            messageSchedule,
                                            parseMode: ParseMode.Html, disableWebPagePreview: true);
                                    }
                                }
                                else
                                {
                                    if (!DbUtils.checkGroup(message.Chat.Id))
                                    {
                                        await botClient.SendTextMessageAsync(
                                            message.Chat.Id,
                                            "Оберіть групу з якою асоціюється цей чат:",
                                            replyMarkup: BotServices.ChooseGroupKeyboard());
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id,
                                            "Група вже зареєстрована! \n \n " +
                                            "Якщо ви хочете змінити групу для цього чату - напишіть адміну бота (@ketronix_dev). А поки краще киньте донат на сервер, або згенеруйте розклад: \n \n" +
                                            "/schedule - на сьогодні \n" +
                                            "/schedule_week - на тиждень");
                                    }
                                }
                            }

                            if (message.Chat.Type != ChatType.Private)
                            {
                                if (message.Text == "/register" ||
                                    message.Text == "/register@" + botClient.GetMeAsync().Result.Username)
                                {
                                    if (!DbUtils.checkGroup(message.Chat.Id))
                                    {
                                        await botClient.SendTextMessageAsync(
                                            message.Chat.Id,
                                            "Оберіть групу з якою асоціюється цей чат:",
                                            replyMarkup: BotServices.ChooseGroupKeyboard());
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id,
                                            "Група вже зареєстрована! \n \n " +
                                            "Якщо ви хочете змінити групу для цього чату - напишіть адміну бота (@ketronix_dev). А поки краще киньте донат на сервер, або згенеруйте розклад: \n \n" +
                                            "/schedule - на сьогодні \n" +
                                            "/schedule_week - на тиждень");
                                    }
                                }

                                if (message.Text == "/schedule" ||
                                    message.Text == "/schedule@" + botClient.GetMeAsync().Result.Username)
                                {
                                    if (DbUtils.checkGroup(message.Chat.Id))
                                    {
                                        var events = Schedule.GetCistEvents(
                                            Schedule.GetCistShedule(DbUtils.GetGroup(message.Chat.Id), Key),
                                            DateService.GetToday(),
                                            DateService.GetToday());
                                        var messageSchedule = MessageGenerator.GenerateMessageForToday(
                                            events,
                                            DbUtils.GetGroup(message.Chat.Id));

                                        if (events.Count is not 0)
                                        {
                                            await botClient.SendTextMessageAsync(
                                                message.Chat.Id,
                                                messageSchedule,
                                                parseMode: ParseMode.Html, disableWebPagePreview: true);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(
                                                message.Chat.Id,
                                                "Пар сьогодні нема, дозволяю відпочити");
                                        }
                                    }
                                }

                                if (message.Text == "/schedule_week" ||
                                    message.Text == "/schedule_week@" + botClient.GetMeAsync().Result.Username)
                                {
                                    if (DbUtils.checkGroup(message.Chat.Id))
                                    {
                                        var weekDates = DateService.GetWeekDates(DateService.GetToday());
                                        var messageSchedule = MessageGenerator.GenerateMessageForWeek(
                                            DbUtils.GetGroup(message.Chat.Id), weekDates[0], weekDates[1], Key);

                                        await botClient.SendTextMessageAsync(
                                            message.Chat.Id,
                                            messageSchedule,
                                            parseMode: ParseMode.Html, disableWebPagePreview: true);
                                    }
                                }
                            }
                        }
                    }

                    if (message.NewChatMembers is not null)
                    {
                        if (message.NewChatMembers.Any(x => x.Id == botClient.GetMeAsync().Result.Id))
                        {
                            if (DbUtils.checkBlockGroup(message.Chat.Id))
                            {
                                await botClient.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Ця група була заблокована адміністратором бота, за подробицями пишіть адміністратору бота." +
                                    "\n \n Будь-ласка, не намагайтеся написати повідомлення для адміністратора в приватну повідомлення боту, адміністратор все одно їх так не побачить.");
                            }
                            else
                            {
                                if (!DbUtils.checkGroup(message.Chat.Id))
                                {
                                    join = await botClient.SendTextMessageAsync(
                                        message.Chat.Id,
                                        "Для роботи боту треба бути адміном з мінімальними правами (а адміну ботом) " +
                                        "Після надання прав адміна - тисніть кнопку нижче \n \n" +
                                        "Якщо нічого не взірвалось, відправилось, чи не надійшло адміну бота на картку - це означає шо бот адмін права не отримав. " +
                                        "Якщо бот отримав адмінку, то він вам про це скаже.",
                                        replyMarkup: BotServices.IsAdminKeyboard());
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Я знову тут! \n \n " +
                                        "Для генерації розкладу використовуйте команди нижче: \n \n" +
                                        "/schedule - на сьогодні \n" +
                                        "/schedule_week - на тиждень");
                                }
                            }
                        }
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await BotServices.HandleCallbackQuery(botClient, update.CallbackQuery);
                    await BotServices.HandleIsAdmin(botClient, update.CallbackQuery, join);
                }
            }
            catch (Exception e)
            {
                await ExecuteCode(botClient, update);
                throw;
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            try
            {
                await botClient.SendTextMessageAsync(
                    -1001638301850,
                    Newtonsoft.Json.JsonConvert.SerializeObject(exception, Formatting.Indented));
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(
                    -1001638301850,
                    Newtonsoft.Json.JsonConvert.SerializeObject(e, Formatting.Indented));
            }
        }


        static void Main(string[] args)
        {
            if (!File.Exists("config-bot.toml"))
            {
                Console.WriteLine(
                    "A token is necessary for the work of the bot. To get the token, you need to create a bot here: @BotFather\nThe mysql database is also required for work.");
                Console.Write("Please enter the token from the bot: ");
                string tokenBot = Console.ReadLine();
                Console.Write("\nSpecify the address to the database: ");
                string addressDatabase = Console.ReadLine();
                Console.Write("Specify the name of the database: ");
                string nameDatabase = Console.ReadLine();
                Console.Write("Now the username: ");
                string nameUserDatabase = Console.ReadLine();
                Console.Write("And the password: ");
                string passwordUserDatabase = Console.ReadLine();
                Console.Write("NURE CIST API key: ");
                string apiKey = Console.ReadLine();

                using (FileStream fstream = new FileStream("config-bot.toml", FileMode.OpenOrCreate))
                {
                    string configText =
                        String.Format(
                            "botToken = '{0}'\naddressDatabase = '{1}'\nnameDatabase = '{2}'\nnameUserDatabase = '{3}'\npasswordUserDatabase = '{4}'\napiKey = '{5}",
                            tokenBot,
                            addressDatabase,
                            nameDatabase,
                            nameUserDatabase,
                            passwordUserDatabase,
                            apiKey);
                    Console.WriteLine(configText);
                    byte[] buffer = Encoding.Default.GetBytes(configText);
                    fstream.Write(buffer, 0, buffer.Length);
                }
            }

            string logFilePath = "log.txt";
            FileStream logFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            StreamWriter logStreamWriter = new StreamWriter(logFileStream);

            Console.SetOut(logStreamWriter); // Встановлюємо StreamWriter як потік виводу консолі


            using (FileStream fstream = File.OpenRead("config-bot.toml"))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);

                var model = Toml.ToModel(textFromFile);
                bot = new TelegramBotClient((string)model["botToken"]!);
                Key = (string)model["apiKey"];
            }

            DbUtils.CreateTableOrNo();

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
            logStreamWriter.Flush();
            logFileStream.Flush();
            logStreamWriter.Close();
            logFileStream.Close();
        }
    }
}
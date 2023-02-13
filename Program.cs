using Telegram.Bot;
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
        private static bool OnAnswers = false;
        private static int state = 0;
        private static Message register;
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
                Console.WriteLine("Code execution failed: " + ex.Message);
            }
            
            Thread.Sleep(3000);
        }
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            await ExecuteCode(botClient, update);
            try
            {
                DbUtils.CreateTableOrNo();
                if (update.Type == UpdateType.Message)
                {
                    /*Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update, Formatting.Indented));*/
                    var message = update.Message;

                    if (message.NewChatMembers is not null)
                    {
                        if (message.NewChatMembers.Any(x => x.Id == botClient.GetMeAsync().Result.Id))
                        {
                            if (!DbUtils.checkGroup(message.Chat.Id))
                            {
                                join = await botClient.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Для работы боту нужно иметь минимальные права администратора. " +
                                    "После того как вы будете уверены в том что он их получил - нажмите кнопку ниже. \n \n" +
                                    "Если после этого ничего не произошло - это означает что проверка на наличие прав не была пройдена успешно. " +
                                    "Если проверка пройдет успешно - бот об этом уведомит с дальнейшей инструкцией для активацией бота.",
                                    replyMarkup: BotServices.IsAdminKeyboard());
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Я снова туточки! \n \n " +
                                    "Если вы хотите чтобы я вам отправил сообщение с расписанием, используйте команды которые написаны ниже: \n \n" +
                                    "/schedule - получить расписание на сегодня. \n" +
                                    "/schedule_week - получить расписание на всю неделю.");
                            }
                        }
                    }

                    if (message.Chat.Type == ChatType.Private)
                    {
                        if (DbUtils.checkGroup(message.Chat.Id))
                        {
                            if (message.Text == "/schedule" ||
                                message.Text == "/schedule@" + botClient.GetMeAsync().Result.Username)
                            {
                                var events = Schedule.GetCistEvents(
                                    Schedule.GetCistShedule(DbUtils.GetGroup(message.Chat.Id)),
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
                                        "Пар сегодня нет, расслабьтесь.");
                                }
                            }

                            if (message.Text == "/schedule_week" ||
                                message.Text == "/schedule_week@" + botClient.GetMeAsync().Result.Username)
                            {
                                var weekDates = DateService.GetWeekDates(DateService.GetToday());
                                var messageSchedule = MessageGenerator.GenerateMessageForWeek(
                                    DbUtils.GetGroup(message.Chat.Id), weekDates[0], weekDates[1]);

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
                                register = await botClient.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Выберите группу с которой ассоциируется данный чат:",
                                    replyMarkup: BotServices.ChooseGroupKeyboard());
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    "Группа уже зарегистрирована! \n \n " +
                                    "Если вы хотите изменить группу к которой относится этот чат - напишите администратору (@ketronix_dev). А пока вы можете получить расписание по командам ниже: \n \n" +
                                    "/schedule - получить расписание на сегодня. \n" +
                                    "/schedule_week - получить расписание на всю неделю.");
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
                                register = await botClient.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Выберите группу с которой ассоциируется данный чат:",
                                    replyMarkup: BotServices.ChooseGroupKeyboard());
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    "Группа уже зарегистрирована! \n \n " +
                                    "Если вы хотите изменить группу к которой относится этот чат - напишите администратору (@ketronix_dev). А пока вы можете получить расписание по командам ниже: \n \n" +
                                    "/schedule - получить расписание на сегодня. \n" +
                                    "/schedule_week - получить расписание на всю неделю.");
                            }
                        }

                        if (message.Text == "/schedule" ||
                            message.Text == "/schedule@" + botClient.GetMeAsync().Result.Username)
                        {
                            if (DbUtils.checkGroup(message.Chat.Id))
                            {
                                var events = Schedule.GetCistEvents(
                                    Schedule.GetCistShedule(DbUtils.GetGroup(message.Chat.Id)),
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
                                        "Пар сегодня нет, расслабьтесь.");
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
                                    DbUtils.GetGroup(message.Chat.Id), weekDates[0], weekDates[1]);

                                await botClient.SendTextMessageAsync(
                                    message.Chat.Id,
                                    messageSchedule,
                                    parseMode: ParseMode.Html, disableWebPagePreview: true);
                            }
                        }
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await BotServices.HandleCallbackQuery(botClient, update.CallbackQuery, register);
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

                using (FileStream fstream = new FileStream("config-bot.toml", FileMode.OpenOrCreate))
                {
                    string configText =
                        String.Format(
                            "botToken = '{0}'\naddressDatabase = '{1}'\nnameDatabase = '{2}'\nnameUserDatabase = '{3}'\npasswordUserDatabase = '{4}'",
                            tokenBot,
                            addressDatabase,
                            nameDatabase,
                            nameUserDatabase,
                            passwordUserDatabase);
                    Console.WriteLine(configText);
                    byte[] buffer = Encoding.Default.GetBytes(configText);
                    fstream.Write(buffer, 0, buffer.Length);
                }
            }

            using (FileStream fstream = File.OpenRead("config-bot.toml"))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);

                var model = Toml.ToModel(textFromFile);
                bot = new TelegramBotClient((string)model["botToken"]!);
                ;
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
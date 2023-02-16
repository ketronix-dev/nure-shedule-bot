using NureBotSchedule.DatabaseUtils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace NureBotSchedule.Services;

public class BotServices
{
    public static InlineKeyboardMarkup ChooseGroupKeyboard()
    {
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("KIUKI-22-1", "kiuki_22_1"),
                InlineKeyboardButton.WithCallbackData("KIUKI-22-2", "kiuki_22_2"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("KIUKI-22-3", "kiuki_22_3"), 
                InlineKeyboardButton.WithCallbackData("KIUKI-22-4", "kiuki_22_4"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("KIUKI-22-5", "kiuki_22_5"), 
                InlineKeyboardButton.WithCallbackData("KIUKI-22-6", "kiuki_22_6"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("KIUKI-22-7", "kiuki_22_7"), 
                InlineKeyboardButton.WithCallbackData("KIUKI-22-8", "kiuki_22_8"),
            }
        });
        return keyboard;
    }
    public static InlineKeyboardMarkup IsAdminKeyboard()
    {
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Проверить админ ли бот.", "isBot"),
            }
        });
        return keyboard;
    }

    public static async Task GroupChoosed(ITelegramBotClient bot, long MessageChatId, int MessageId, Group group )
    {
        await bot.SendTextMessageAsync(
            MessageChatId,
            $"Теперь в этот чат будет отправляться расписание для группы {group.GroupName}");
        try
        {
            await bot.DeleteMessageAsync(MessageChatId, MessageId);
        }
        catch (Exception e)
        {
            throw;
        }
        DbUtils.InsertGroup(MessageChatId, group.GroupNumber, group.GroupId);
    }
    public static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, Message register)
        {
            if (callbackQuery.Data == "kiuki_22_1")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304320,
                    GroupName = "KIUKI-22-1",
                    GroupNumber = 1
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_2")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304322,
                    GroupName = "KIUKI-22-2",
                    GroupNumber = 2
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_3")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304325,
                    GroupName = "KIUKI-22-3",
                    GroupNumber = 3
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_4")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304327,
                    GroupName = "KIUKI-22-4",
                    GroupNumber = 4
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_5")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304329,
                    GroupName = "KIUKI-22-5",
                    GroupNumber = 5
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_6")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304331,
                    GroupName = "KIUKI-22-6",
                    GroupNumber = 6
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_7")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304333,
                    GroupName = "KIUKI-22-7",
                    GroupNumber = 7
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
            if (callbackQuery.Data == "kiuki_22_8")
            {
                Group group = new Group()
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    GroupId = 10304335,
                    GroupName = "KIUKI-22-8",
                    GroupNumber = 8
                };
                await GroupChoosed(botClient, callbackQuery.Message.Chat.Id, register.MessageId, group);
            }
        }
    
    public static async Task<bool> CheckBotAdminAsync(ITelegramBotClient botClient, long chatID)
    {
        try
        {
            var me = await botClient.GetMeAsync();
            var chatMembers = await botClient.GetChatAdministratorsAsync(chatID);
            return chatMembers.Any(x => x.User.Id == me.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occured while checking bot admin status: " + ex.Message);
            return false;
        }
    }

    public static async Task HandleIsAdmin(ITelegramBotClient botClient, CallbackQuery callbackQuery, Message message)
    {
        if (callbackQuery.Data == "isBot")
        {
            try
            {
                var chatMember = await CheckBotAdminAsync(botClient, message.Chat.Id);

                if (chatMember == true)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Проверка пройдена успешно, для выбора расписания для этого чата выполните команду /register.");
                    try
                    {
                        await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
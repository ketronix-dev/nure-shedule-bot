namespace Bot.Logging;

public class Log
{
    public static string LogMessage(string FirstName, string LastName, string Title, long ChatId, string Text)
    {
        var message = $"{"Firstname:"} {FirstName}\n" +
            $"{"LastName:"} {LastName}\n" +
            $"{"Title:"} {Title}\n" +
            $"{"Id:"} {ChatId}\n" +
            $"{"Text:"} {Text}";
        return message;
    }
}
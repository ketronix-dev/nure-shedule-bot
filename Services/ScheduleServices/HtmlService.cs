using Firebase.Database;
using Firebase.Database.Query;

namespace NureBotSchedule.Services.ScheduleServices;

public class HtmlService
{
    public static FirebaseClient firebase = new FirebaseClient(
        "https://schedulebot-ea3d4-default-rtdb.europe-west1.firebasedatabase.app/");

    public static async Task<string>? SubjectLink(string GroupNumber, string SubjectName, string SubjectType)
    {
        var link = await firebase
            .Child($"Links/{GroupNumber}/{SubjectName}/{SubjectType}")
            .OrderByKey()
            .OnceSingleAsync<string>();

        if (link is not null && link is not "")
        {
            return link;
        }
        else
        {
            return null;
        }
    }

    public static string GetEventHtml(CistEvent i, string GroupNumber)
    {
        var message = "";
        var link = SubjectLink(GroupNumber, i.SubjectShortName, i.EventType);
        if (link.Result is null || link.Result == "")
        {
            message = $"{i.StartTime}-{i.EndTime} | {i.SubjectShortName} - {i.EventType.ToUpper()}" +
                      $"\t <a href=\"\">Посилання нема</a>\n";
            return message;
        }
        switch (i.SubjectShortName)
        {
            case "ІМ":
                switch (i.EventType)
                {
                    case "Пз":

                        message =
                            $"{i.StartTime}-{i.EndTime} | {i.SubjectShortName} - {i.EventType.ToUpper()}" +
                            $"\t <a href=\"{link.Result.Split(" | ")[0]}\">Посилання (вчитель 1)</a> : " +
                            $"<a href=\"{link.Result.Split(" | ")[1]}\">Посилання (Вчитель 2)</a>\n";
                        break;
                }

                break;
            default:
                message = $"{i.StartTime}-{i.EndTime} | {i.SubjectShortName} - {i.EventType.ToUpper()}" +
                          $"\t <a href=\"{link.Result}\">Посилання</a>\n";
                break;
        }

        return message;
    }
}

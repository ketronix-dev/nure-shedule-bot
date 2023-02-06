using System.Globalization;
using Newtonsoft.Json;

public record CistEvent(
    string SubjectShortName,
    string EventType,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);

internal record JsonEvent(
    string Subject,
    string Date,
    string startTime,
    string endTime
);

public class CistParsingException : Exception
{
    public CistParsingException(string message) : base(message)
    {
    }
}

public class NureCistParser
{
    public static List<CistEvent> Parse(int GroupNumber)
    {
        var json = File.ReadAllText($"./course-{GroupNumber}.json");
        List<JsonEvent> jsonEvents = ParseJson(json);
        List<CistEvent> events = new List<CistEvent>();

        foreach (var item in jsonEvents)
        {
            var cistEvent = new CistEvent(
                item.Subject.Split()[0],
                item.Subject.Split()[1],
                DateOnly.ParseExact(item.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                TimeOnly.Parse(item.startTime),
                TimeOnly.Parse(item.endTime)
            );
            events.Add(cistEvent);
        }
        return events.OrderBy
                (@event => @event.Date.ToDateTime(@event.StartTime))
            .ToList();
    }
    private static List<JsonEvent> ParseJson(string json)
    {
        return JsonConvert.DeserializeObject<List<JsonEvent>>(json);
    }
}
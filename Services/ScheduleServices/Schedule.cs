namespace NureBotSchedule.Services.ScheduleServices;

public class Schedule
{
    public static List<CistEvent> GetCistShedule(Group group)
    {
        var result = NureCistParser.Parse(group.GroupNumber);
        return result;
    }
    
    public static List<CistEvent> GetCistEvents(List<CistEvent> events, DateOnly startDate, DateOnly endDate)
    {
        return events.Where(e => e.Date >= startDate && e.Date <= endDate).ToList();
    }
}
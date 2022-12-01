using System.Globalization;

namespace Bot.DateManagment;

public class Date
{
    public static string[] GetWeek()
    {
        DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(
            (int) CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - 
            (int) DateTime.Today.DayOfWeek));

        var result = Enumerable.Range(1, 5).Select(i => startOfWeek
            .AddDays(i)).ToArray();

        var weekArray = new List<string>() { };

        foreach (var day in result)
        {
            weekArray.Add($"{day.Day}.{day.Month}.{day.Year}");
        }
        
        return weekArray.ToArray();
    }

    public static string GetLastDayOnPreviousWeek()
    {
        DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
        DateTime startingDate = DateTime.Today;

        while(startingDate.DayOfWeek != weekStart)
            startingDate = startingDate.AddDays(-1);
        
        DateTime previousWeekEnd = startingDate.AddDays(-1);
        
        return $"{previousWeekEnd.Day}.{previousWeekEnd.Month}.{previousWeekEnd.Year}";
    }
}
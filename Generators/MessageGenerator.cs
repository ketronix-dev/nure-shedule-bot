using NureBotSchedule.DateManagment;
using NureBotSchedule.Services;
using NureBotSchedule.Services.ScheduleServices;

namespace NureBotSchedule.Generators;

public class MessageGenerator
{
    public static string DonateHTML = "\n \n" +
                                      "<a href=\"https://t.me/kiuki_22_botinfo\">Info</a> | " +
                                      "<a href=\"https://osel.pp.ua/donate/\">Donate</a> | " +
                                      "<a href=\"https://t.me/ketronix_dev\">Support</a> | " +
                                      "<a href=\"https://github.com/ketronix-dev/nure-shedule-bot\">Source code</a>" +
                                      "\n";
    public static string GenerateMessageForToday(List<CistEvent> events, Group group)
    {
        string message = "";
        
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        {
            message = "Пар сьогодні нема, дозволяю відпочити";
        }
        else
        {
            message = $"Розклад на: {$"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}"} \n --------------------------- \n";
            foreach (var i in events)
            {
                message += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            }
        }
        return message + DonateHTML;
    }
    
    public static string GenerateMessageForWeek(Group group, DateOnly startDate, DateOnly endDate)
    {
        var result = NureCistParser.Parse(group.GroupNumber);
        
        string startWeek = startDate.ToString("dd.MM.yyyy");
        string endWeek = endDate.ToString("dd.MM.yyyy");
        var message = $"Розклад на: {$"{startWeek} - {endWeek}"} \n -------------------------- \n";

        var monday = $"\n Понеділок | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n";
        var tuesday = $"\n \n Вівторок | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n";
        var wednesday = $"\n \n Середа | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n";
        var thurdday = $"\n \n Четвер | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n";
        var friday = $"\n \n П'ятниця | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n";
        
        foreach (var i in result)
        {
            if (i.Date == DateOnly.ParseExact(DateService.GetWeekDays(startWeek, endWeek)[0], "d.M.yyyy"))
            {
                monday += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            } else if (i.Date == DateOnly.ParseExact(DateService.GetWeekDays(startWeek, endWeek)[1], "d.M.yyyy"))
            {
                tuesday += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            }else if (i.Date == DateOnly.ParseExact(DateService.GetWeekDays(startWeek, endWeek)[2], "d.M.yyyy"))
            {
                wednesday += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            }else if (i.Date == DateOnly.ParseExact(DateService.GetWeekDays(startWeek, endWeek)[3], "d.M.yyyy"))
            {
                thurdday += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            }else if (i.Date == DateOnly.ParseExact(DateService.GetWeekDays(startWeek, endWeek)[4], "d.M.yyyy"))
            {
                friday += HtmlService.GetEventHtml(i, group.GroupNumber.ToString());
            }
        }

        if (monday == $"\n Понеділок | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n")
        {
            monday = $"\n \n Понеділок | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n В цей день пар нема, можна гуляти.";
        }if (tuesday == $"\n \n Вівторок | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n")
        {
            tuesday = $"\n \n Вівторок | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n В цей день пар нема, можна гуляти.";
        }if (wednesday == $"\n \n Середа | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n")
        {
            wednesday = $"\n \n Середа | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n В цей день пар нема, можна гуляти.";
        }if (thurdday == $"\n \n Четвер | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n")
        {
            thurdday = $"\n \n Четвер | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n В цей день пар нема, можна гуляти.";
        }if (friday == $"\n \n П'ятниця | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n")
        {
            friday = $"\n \n П'ятниця | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n В цей день пар нема, можна гуляти.";
        }

        message += monday + tuesday + wednesday + thurdday + friday;
        
        return message + DonateHTML;
    }
}

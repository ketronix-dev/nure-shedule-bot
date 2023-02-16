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
            message = "Пар сегодня нет, росслабьтесь";
        }
        else
        {
            message = $"Расписание на: {$"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}"} \n --------------------------- \n";
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
        var message = $"Расписание на: {$"{startWeek} - {endWeek}"} \n -------------------------- \n";

        var monday = $"\n Понедельник | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n";
        var tuesday = $"\n \n Вторник | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n";
        var wednesday = $"\n \n Среда | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n";
        var thurdday = $"\n \n Четверг | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n";
        var friday = $"\n \n Пятница | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n";
        
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

        if (monday == $"\n Понедельник | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n")
        {
            monday = $"\n \n Понедельник | {DateService.GetWeekDays(startWeek, endWeek)[0]} \n В этот день пар нет.";
        }if (tuesday == $"\n \n Вторник | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n")
        {
            tuesday = $"\n \n Вторник | {DateService.GetWeekDays(startWeek, endWeek)[1]} \n В этот день пар нет.";
        }if (wednesday == $"\n \n Среда | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n")
        {
            wednesday = $"\n \n Среда | {DateService.GetWeekDays(startWeek, endWeek)[2]} \n В этот день пар нет.";
        }if (thurdday == $"\n \n Четверг | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n")
        {
            thurdday = $"\n \n Четверг | {DateService.GetWeekDays(startWeek, endWeek)[3]} \n В этот день пар нет.";
        }if (friday == $"\n \n Пятница | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n")
        {
            friday = $"\n \n Пятница | {DateService.GetWeekDays(startWeek, endWeek)[4]} \n В этот день пар нет.";
        }

        message += monday + tuesday + wednesday + thurdday + friday;
        
        return message + DonateHTML;
    }
}
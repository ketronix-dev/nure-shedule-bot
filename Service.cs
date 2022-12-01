using System.Globalization;
using System.Text;
using HtmlAgilityPack;
using Bot.DateManagment;

public class Service
{
    public static async Task GenerateHtmLforShedule(string startDate, string endDate, bool week)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var html = $"https://cist.nure.ua/ias/app/tt/f?p=778:201:2925093908151803:::201:P201_FIRST_DATE,P201_LAST_DATE,P201_GROUP,P201_POTOK:{startDate},{endDate},10304333,0:";

        HtmlWeb web = new HtmlWeb();
        var htmlDoc = await web.LoadFromWebAsync(html);

        var node = htmlDoc.DocumentNode.SelectSingleNode("/");
        if(week)
            File.WriteAllText($"./week-{endDate}.html", node.OuterHtml);
        else
            File.WriteAllText($"./day-{startDate}.html", node.OuterHtml);
        /*Console.WriteLine(node.OuterHtml);*/
    }

    public static string GetEventHtml(CistEvent i)
    {
        var message = "";
        switch (i.SubjectShortName)
                {
                    case "ФВ":
                        message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                   $"\t <a href=\"https://meet.google.com/buz-atjo-mpe\">Ссылка</a>\n";
                        break;
                    case "БЖД":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/vns-vuyf-daj\">Ссылка</a>\n";
                                break;
                            case "Лб":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ЛАБА" +
                                           $"\t <a href=\"https://meet.google.com/vns-vuyf-daj\">Ссылка</a>\n";
                                break;
                            case "Пз":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ЛАБА" +
                                           $"\t <a href=\"\">Ссылка еще не добавлена</a>\n";
                                break;
                        }
                        break;
                    case "ВМ":
                        message += $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                   $"\t <a href=\"https://meet.google.com/qzi-dnak-fdv\">Ссылка</a>\n";
                        break;
                    case "УФМ":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/ciz-vqce-gbi\">Ссылка</a>\n";
                                break;
                            case "Пз":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ПЗ" +
                                           $"\t <a href=\"https://meet.google.com/jtu-mntd-nev?authuser=0&hs=179\">Ссылка</a>\n";
                                break;
                            
                        }
                        break;
                    case "Про":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/zen-hngd-tnj\">Ссылка</a>\n";
                                break;
                            case "Лб":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ЛАБА" +
                                           $"\t <a href=\"https://meet.google.com/sbp-jfrd-bhz\">Ссылка</a>\n";
                                break;
                        }
                        break;
                    case "Фіз":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/eds-dtko-rrx\">Ссылка</a>\n";
                                break;
                            case "Лб":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ЛАБА" +
                                           $"\t <a href=\"https://meet.google.com/vwx-udxg-cuz\">Ссылка</a>\n";
                                break;
                            case "Пз":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ПЗ" +
                                           $"\t <a href=\"https://meet.google.com/coe-uytn-twm\">Ссылка</a>\n";
                                break;
                        }
                        break;
                    case "ОПтФОС":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/yxp-uanv-ycg\">Ссылка</a>\n";
                                break;
                            case "Лб":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ЛАБА" +
                                           $"\t <a href=\"https://meet.google.com/gtu-btzw-bic\">Ссылка</a>\n";
                                break;
                        }
                        break;
                    case "ІМ":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a href=\"https://meet.google.com/ehd-pfdv-pfi\">Ссылка (Сіз)</a>:<a href=\"https://meet.google.com/ayc-djzp-znn\">Ссылка (Бук)</a>\n";
                                break;
                            case "Пз":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ПЗ" +
                                           $"\t <a href=\"https://meet.google.com/ehd-pfdv-pfi\">Ссылка (Сіз)</a>:<a href=\"https://meet.google.com/ayc-djzp-znn\">Ссылка (Бук)</a>\n";
                                break;
                        }
                        break;
                    case "ДМ":
                        switch (i.EventType)
                        {
                            case "Лк":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                           $"\t <a>Ссылка на почте</a>\n";
                                break;
                            case "Пз":
                                message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName} - ПЗ" +
                                           $"\t <a>Ссылка на почте</a>\n";
                                break;
                            
                        }
                        break;
                    default:
                        message = $"{i.Number} | {i.StartTime}-{i.EndTime} | {i.SubjectShortName}" +
                                   $"\t <a>Ссылка еще не добавлена</a>\n";
                        break;
                }

        return message;
    }

    public static string GetCistShedule(string pathToFile)
    {
        string message = "";
        
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        {
            message = "Пар сегодня нет, росслабьтесь";
        }
        else
        {
            var html = File.OpenRead(pathToFile);
            var parser = new CistHtmlParser();
            var result = parser.ParseAsSequence(html).ToList();
            
            message = $"Расписание на: {$"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}"} \n ---------------------------------------------------------------- \n";
            foreach (var i in result)
            {
                message += GetEventHtml(i);
            }
        }
        return message;
    }

    public static string GetWeekShedule(string pathToFile)
    {
        var html = File.OpenRead(pathToFile);
        var parser = new CistHtmlParser();
        var result = parser.ParseAsSequence(html).ToList();

        var week = Date.GetWeek();

        var message = $"Расписание на: {$"{week[0]} - {week[4]}"} \n --------------------------------------- \n";

        var monday = $"\n \n Понедельник | {week[0]} \n";
        var tuesday = $"\n \n Вторник | {week[1]} \n";
        var wednesday = $"\n \n Среда | {week[2]} \n";
        var thurdday = $"\n \n Четверг | {week[3]} \n";
        var friday = $"\n \n Пятница | {week[4]} \n";
        
        foreach (var i in result)
        {
            if (i.Date == DateOnly.ParseExact(week[0], "d.M.yyyy"))
            {
                monday += GetEventHtml(i);
            } else if (i.Date == DateOnly.ParseExact(week[1], "d.M.yyyy"))
            {
                tuesday += GetEventHtml(i);
            }else if (i.Date == DateOnly.ParseExact(week[2], "d.M.yyyy"))
            {
                wednesday += GetEventHtml(i);
            }else if (i.Date == DateOnly.ParseExact(week[3], "d.M.yyyy"))
            {
                thurdday += GetEventHtml(i);
            }else if (i.Date == DateOnly.ParseExact(week[3], "d.M.yyyy"))
            {
                friday += GetEventHtml(i);
            }
        }

        message += monday + tuesday + wednesday + thurdday + friday;
        
        return message;
    }
}
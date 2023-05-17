using NureBotSchedule.ServiceClasses;

namespace NureBotSchedule.CistServices;

public class EventComposer
{
    public static Teacher? FindTeacherById(Teacher[] teachers, int id)
    {
        foreach (Teacher teacher in teachers)
        {
            if (teacher.id == id)
            {
                return teacher;
            }
        }

        return null; // якщо клас з таким ідентифікатором не знайдено
    }

    public static Subject? FindSubjectById(Subject[] subjects, int id)
    {
        foreach (Subject subject in subjects)
        {
            if (subject.id == id)
            {
                return subject;
            }
        }

        return null; // якщо клас з таким ідентифікатором не знайдено
    }

    public static string GetEventType(int id)
    {
        if (id == 10)
        {
            return "Пз";
        }
        else if (id == 20)
        {
            return "Лб";
        }
        else if (id == 30)
        {
            return "Конс";
        }
        else if (id == 40)
        {
            return "Зал";
        }
        else if (id == 50)
        {
            return "Екз";
        }

        return "Лк";
    }

    public static CistEvent[] GetEvents(Schedule schedule)
    {
        List<CistEvent> events = new List<CistEvent>();

        foreach (var item in schedule.events)
        {
            var timeAndDateStart = DateTimeOffset.FromUnixTimeSeconds(item.start_time);
            var timeAndDateEnd = DateTimeOffset.FromUnixTimeSeconds(item.end_time);
            var cistEvent = new CistEvent()
            {
                number_pair = item.number_pair,
                subject = FindSubjectById(schedule.Subjects, item.subject_id),
                date = DateOnly.FromDateTime(timeAndDateStart.LocalDateTime),
                start_time = TimeOnly.FromDateTime(timeAndDateStart.LocalDateTime),
                end_time = TimeOnly.FromDateTime(timeAndDateEnd.LocalDateTime),
                type = GetEventType(item.type),
                teachers = new List<Teacher>()
            };

            if (cistEvent.subject.id == 8051836)
            {
                cistEvent.teachers.Add(new Teacher()
                {
                    full_name = "Не зазначено",
                    short_name = "Не зазначено",
                    id = 12345
                });
            }
            else
            {
                foreach (var teacher in item.teachers)
                {
                    var teach = FindTeacherById(schedule.teachers, teacher);

                    cistEvent.teachers.Add(teach);
                }
            }

            events.Add(cistEvent);
        }

        return events.ToArray();
    }
}
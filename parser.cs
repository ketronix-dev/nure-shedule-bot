using System.Text;
using HtmlAgilityPack;

public record CistEvent(
  string SubjectShortName,    
  string EventType,           
  string Place,               
  DateOnly Date,              
  int Number,                 
  TimeOnly StartTime,         
  TimeOnly EndTime            
);

public class CistParsingException : Exception
{
  public CistParsingException(string message) : base(message)
  {
  }
}

public class CistHtmlParser
{
  private const string MainTableClassName = "MainTT";
  private const string FooterTableClassName = "footer";

  
  public IReadOnlyCollection<CistEvent> ParseAsSequence(Stream timeTableHtml) =>
    ParseInternal(timeTableHtml)
      .OrderBy(@event => @event.Date.ToDateTime(@event.StartTime))
      .ToList();

  private IEnumerable<CistEvent> ParseInternal(Stream timeTableHtml)
  {
    var htmlDocument = new HtmlDocument();
    htmlDocument.Load(timeTableHtml);

    var mainTable = htmlDocument.DocumentNode
      .SelectNodes($"//table[@class='{MainTableClassName}']")
      .FirstOrDefault();

    if (mainTable is null)
    {
      throw new CistParsingException($"Invalid input: A table with '{MainTableClassName}' class is not presented.");
    }

    var state = new ParserState();
    foreach (var row in mainTable.ChildNodes.Where(node => node.Name is "tr"))
    {
      var firstChild = row.ChildNodes.First(node => node.Name is "td");
      if (firstChild.HasClass("date"))
      {
        ProcessDaysOriginsRow(row, state);
        continue;
      }

      if (firstChild.HasClass("left"))
      {
        foreach (var @event in ProcessEventDataRow(row, state))
        {
          yield return @event;
        }
      }
    }

    var footerTable = htmlDocument.DocumentNode.SelectNodes($"//table[@class='{FooterTableClassName}']").FirstOrDefault();
    if (footerTable is null)
    {
      throw new CistParsingException("Invalid input: A table with 'MainTT' class is not presented.");
    }
  }
  
  private void ProcessDaysOriginsRow(HtmlNode row, ParserState state)
  {
    state.DayOfWeek++;
    
    state.DaysOrigins = row
      .ChildNodes
      .Where(node => node.Name is "td")
      .Skip(1)
      .Select(node => DateOnly.ParseExact(node.InnerText, "dd.MM.yyyy"))
      .ToArray();
  }
  
  private IEnumerable<CistEvent> ProcessEventDataRow(HtmlNode row, ParserState state)
  {
    var columns = row.ChildNodes.Where(node => node.Name is "td").ToArray();
    var eventNumber = int.Parse(columns[0].InnerText.Trim());

    
    var startEndTime = columns[1].InnerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var startTime = TimeOnly.ParseExact(startEndTime[0], "HH:mm");
    var endTime = TimeOnly.ParseExact(startEndTime[1], "HH:mm");
    for (var i = 0; i < columns.Length - 2; i++)
    {
      
      var column = columns[i + 2];

      
      if (column.InnerText.Trim() == "&nbsp") continue;

      var subjectAndTypeLink = column.ChildNodes.First(node => node.Name is "a");
      var subjectAndType = subjectAndTypeLink.InnerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      var subject = subjectAndType[0];
      var type = subjectAndType[1];
      var place = subjectAndTypeLink.NextSibling.InnerText.Trim();

      
      var duplicatesCount = column.GetAttributeValue("colspan", 1);
      for (var j = 0; j < duplicatesCount; j++)
      {
        var weekNumber = i + j;
        yield return new CistEvent(subject, type, place, state.DaysOrigins[weekNumber], eventNumber, startTime, endTime);
      }

      
      i += duplicatesCount - 1;
    }
  }

  private class ParserState
  {
    public int DayOfWeek { get; set; } = -1;
    public DateOnly[] DaysOrigins { get; set; } = null!;
  }
}
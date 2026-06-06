namespace UBB_SE_2026_Jobs.Library.DTOs.TI;

public class CalendarCell
{
    public TiSlotDto? Slot { get; set; }
    public bool IsEmpty => Slot == null;
    public int DayIndex { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
}

public class CalendarRow
{
    public string TimeLabel { get; set; } = string.Empty;
    public List<CalendarCell> Cells { get; set; } = new();
}

namespace WebAPI.Helper;

public static class TimeConvert
{
    public static DateTime? Asia(DateTime time)
    {
        return TimeZoneInfo.ConvertTime(time, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    }
}
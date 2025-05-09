using Fatagram.Shared.Enums;

namespace Fatagram.Shared.Extensions 
{
    public class TimeDistance 
    {
        public TimeDistance(int value, TimeUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public int Value { get; }
        public TimeUnit Unit { get; }
    }

    public static class TimeExtensions 
    {
        public static TimeDistance ToTimeDistance(this DateTime dateTime, DateTime targetTime)
        {
            var timeSpan = targetTime - dateTime;

            if (timeSpan.TotalMilliseconds < 0)
            {
                return new TimeDistance(0, TimeUnit.Miliseconds);
            }

            if (timeSpan.TotalSeconds < 60)
            {
                return new TimeDistance((int)timeSpan.TotalSeconds, TimeUnit.Seconds);
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                return new TimeDistance((int)timeSpan.TotalMinutes, TimeUnit.Minutes);
            }
            else if (timeSpan.TotalHours < 24)
            {
                return new TimeDistance((int)timeSpan.TotalHours, TimeUnit.Hours);
            }
            else if (timeSpan.TotalDays < 30)
            {
                return new TimeDistance((int)timeSpan.TotalDays, TimeUnit.Days);
            }
            else if (timeSpan.TotalDays < 365)
            {
                return new TimeDistance((int)(timeSpan.TotalDays / 7), TimeUnit.Weeks);
            }
            else
            {
                return new TimeDistance((int)(timeSpan.TotalDays / 365), TimeUnit.Years);
            }
        } 
    }
}
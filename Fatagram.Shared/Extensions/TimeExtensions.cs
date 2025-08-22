using System.Text.Json.Serialization;
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

        [JsonConverter(typeof(JsonStringEnumConverter))] 
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
        
        public static DateTime ToDateTime(this TimeDistance timeDistance)
        {
            return timeDistance.Unit switch
            {
                TimeUnit.Miliseconds => DateTime.UtcNow.AddMilliseconds(-timeDistance.Value),
                TimeUnit.Seconds => DateTime.UtcNow.AddSeconds(-timeDistance.Value),
                TimeUnit.Minutes => DateTime.UtcNow.AddMinutes(-timeDistance.Value),
                TimeUnit.Hours => DateTime.UtcNow.AddHours(-timeDistance.Value),
                TimeUnit.Days => DateTime.UtcNow.AddDays(-timeDistance.Value),
                TimeUnit.Weeks => DateTime.UtcNow.AddDays(-timeDistance.Value * 7),
                TimeUnit.Months => DateTime.UtcNow.AddMonths(-timeDistance.Value),
                TimeUnit.Years => DateTime.UtcNow.AddYears(-timeDistance.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(timeDistance.Unit), "Invalid time unit")
            };
        }  
    }
}
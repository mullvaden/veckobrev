using System;

namespace DownloadWeekly
{
    public class Clock: IClock
    {
        public DateTime Now { get { return DateTime.Now; } }
    }

    public interface IClock
    {
        DateTime Now { get; }
    }
}
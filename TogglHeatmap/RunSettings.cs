using System;

namespace csMACnz.TogglHeatmap
{
    public class RunSettings
    {
        public RunSettings(string fileName, DateTime startDate, int weekCount)
        : this(fileName, startDate, ResolveEndDate(startDate, weekCount), weekCount)
        {
        }

        public RunSettings(string fileName, DateTime startDate, DateTime endDate)
            : this(fileName, startDate, endDate, ResolveWeekCount(startDate, endDate))
        {
        }

        private RunSettings(string fileName, DateTime startDate, DateTime endDate, int numberOfWeeks)
        {
            FileName = fileName;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfWeeks = numberOfWeeks;
        }

        private static DateTime ResolveEndDate(DateTime startDate, int weekCount)
        {
            return startDate.AddDays(weekCount * 7);
        }

        private static int ResolveWeekCount(DateTime startDate, DateTime endDate)
        {
            return (int)((endDate - startDate).TotalDays / 7);
        }

        public string FileName { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int NumberOfWeeks { get; }
    }
}
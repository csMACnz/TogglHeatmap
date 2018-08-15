using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace csMACnz.TogglHeatmap
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var result = RunMain(args);
            return (int)result;
        }

        public static RunResult RunMain(string[] args)
        {
            return ArgsParser.ParseArgs(args)
                .Match(runResult =>
                {
                    return runResult;
                },
                settings =>
                {
                    return RunApp(settings);
                });
        }

        public static RunResult RunApp(RunSettings settings)
        {
            if (!File.Exists(settings.FileName))
            {
                Console.Error.WriteLine($"Could not find file '{settings.FileName}'");
                return RunResult.FileNotFound;
            }

            var data = new List<TimeEntryData>();
            using (Stream stream = File.OpenRead(settings.FileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var row = line.Split(',');
                        var startDate = row[7];
                        var startTime = row[8];
                        var endDate = row[9];
                        var endTime = row[10];
                        var duration = row[11];
                        DateTime start = DateTime.Parse($"{startDate} {startTime}");
                        DateTime end = DateTime.Parse($"{endDate} {endTime}");

                        data.Add(new TimeEntryData { Start = start, End = end, Duration = duration });
                    }
                }
            }
            var sorted = new Dictionary<DateTime, List<TimeEntryData>>();
            foreach (var dataItem in data)
            {
                AddItem(sorted, dataItem.Start.Date, dataItem);
                if (dataItem.Start.Date != dataItem.End.Date)
                {
                    var date = dataItem.Start.Date;
                    while (date < dataItem.End.Date)
                    {
                        date = date.AddDays(1);
                        AddItem(sorted, date.Date, dataItem);
                    }
                }
            }

            Console.WriteLine($"Data from {settings.StartDate} to {settings.EndDate}");
            string firstDayOfWeek = Enum.GetName(typeof(DayOfWeek), settings.StartDate.DayOfWeek);
            Console.WriteLine($"First day: {firstDayOfWeek}");
            Console.WriteLine($"Number of weeks: {settings.NumberOfWeeks}");
            Console.WriteLine("----");
            var weekdayNames = Enum.GetNames(typeof(DayOfWeek));
            var pivotIndex = weekdayNames
                .Select((v, i) => (index: i, day: v))
                .Where(t => t.day == firstDayOfWeek)
                .Select(t => t.index)
                .First();
            var daysList = new[] { "Time" }
                .Union(weekdayNames.Skip(pivotIndex).Take(7 - pivotIndex))
                .Union(weekdayNames.Skip(7 - pivotIndex).Take(pivotIndex));
            Console.WriteLine(string.Join(",", daysList));

            var allWeeksData = Enumerable
                .Range(0, settings.NumberOfWeeks)
                .Select(i => settings.StartDate.AddDays(i * 7))
                .Select(date => GetData(date, sorted))
                .ToList();

            var results = new int[7, 48];

            foreach (var week in allWeeksData)
            {
                foreach (var day in week.Select((x, i) => new { WeekDay = i, Value = x }))
                {
                    foreach (var segment in day.Value.Select((x, i) => new { WeekDay = day.WeekDay, Halfhour = i, HasData = x }))
                    {
                        if (segment.HasData)
                        {
                            results[segment.WeekDay, segment.Halfhour] += 1;
                        }
                    }
                }
            }

            //Report
            for (var halfHour = 0; halfHour < 48; halfHour++)
            {
                var values = new List<string>();
                values.Add($"{halfHour / 2:00}:{(halfHour % 2 == 0 ? 0 : 30):00}");
                for (int day = 0; day < 7; day++)
                {
                    var count = results[day, halfHour];
                    values.Add(count.ToString());
                }
                Console.WriteLine(string.Join(",", values));
            }
            Console.WriteLine("----");
            Console.WriteLine();
            Console.WriteLine($"Copy the data above into a csv file to make it pretty in a spreadsheet");
            return 0;
        }

        private static bool[][] GetData(DateTime startDate, Dictionary<DateTime, List<TimeEntryData>> data)
        {
            return Enumerable
              .Range(0, 7)
              .Select(i => startDate.AddDays(i))
              .Select(
                  day => ThirtyMinuteSegments()
                    .Take(48)
                    .Select(t => day.Add(t))
                    .Select(d => WasWorking(d, data))
                    .ToArray())
              .ToArray();
        }

        private static bool WasWorking(DateTime range, Dictionary<DateTime, List<TimeEntryData>> data)
        {
            if (data.ContainsKey(range.Date))
            {
                return data[range.Date].Any(d => range.AddMinutes(30) >= d.Start && range <= d.End);
            }
            return false;
        }

        public static void AddItem(Dictionary<DateTime, List<TimeEntryData>> sorted, DateTime key, TimeEntryData dataItem)
        {
            if (!sorted.ContainsKey(key))
            {
                sorted[key] = new List<TimeEntryData>();
            }
            sorted[key].Add(dataItem);
        }

        public static IEnumerable<TimeSpan> ThirtyMinuteSegments()
        {
            TimeSpan time = TimeSpan.Zero;
            do
            {
                yield return time;
                time = time.Add(TimeSpan.FromMinutes(30));
            } while (true);
        }
    }
}

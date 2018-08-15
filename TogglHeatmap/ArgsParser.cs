using System;
using System.Reflection;
using Beefeater;

namespace csMACnz.TogglHeatmap
{
    public static class ArgsParser
    {
        public static Either<RunResult, RunSettings> ParseArgs(string[] args)
        {
            if (args.Length < 1 || args.Length > 3)
            {
                Console.Error.WriteLine("Invalid Arguments");
                PrintUsage();
                return RunResult.InvalidArgs;
            }
            else if (args.Length == 1 && args[0].StartsWith("--"))
            {
                var argument = args[0];
                switch (argument)
                {
                    case "--help":
                        PrintUsage();
                        return RunResult.Ok;
                    case "--version":
                        Console.WriteLine(GetDisplayVersion());
                        return RunResult.Ok;
                    default:
                        Console.Error.WriteLine($"Unknown Argument '{argument}'");
                        PrintUsage();
                        return RunResult.UnknownArgument;
                }
            }
            else if (args.Length != 3)
            {
                Console.Error.WriteLine("Invalid Arguments");
                PrintUsage();
                return RunResult.InvalidArgs;
            }
            var filePath = args[0];
            var startDateInput = args[1];
            DateTime actualStartDate;
            if (DateTime.TryParse(startDateInput, out actualStartDate))
            {
                actualStartDate = actualStartDate.Date;
            }
            else
            {
                Console.Error.WriteLine($"Invalid Start Date '{startDateInput}'");
                PrintUsage();
                return RunResult.IncorrectArgsFormat;
            }

            var endRangeInput = args[2];
            RunSettings settings;
            if (DateTime.TryParse(endRangeInput, out var finalDate))
            {
                settings = new RunSettings(filePath, actualStartDate, finalDate.Date);
            }
            else if (int.TryParse(endRangeInput, out var numberOfWeeks))
            {
                settings = new RunSettings(filePath, actualStartDate, numberOfWeeks);
            }
            else
            {
                Console.Error.WriteLine($"The argument '{endRangeInput}' must be either a Number (week count) or a DateTime (range end date)");
                PrintUsage();
                return RunResult.IncorrectArgsFormat;
            }

            return settings;
        }

        private static string GetDisplayVersion()
        {
            return Assembly
                .GetEntryAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                .Version;
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"TogglHeatmap - Heatmap data generation from toggl time entry data

Usage:
TogglHeatmap dataFile.csv
TogglHeatmap --help
TogglHeatmap --version");
        }

    }
}
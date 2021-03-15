using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Internatura
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("Data/acme_worksheet.csv");
            var employeeWorkingTime = new Dictionary<string, List<WorkingTime>>();
            var dates = new List<DateTime>();

            foreach (var line in lines.Skip(1))
            {
                var split = line.Split(',');

                var fullName = split[0];
                var date = DateTime.ParseExact(split[1], "MMM dd yyyy", new CultureInfo("en-US"));
                var hours = decimal.Parse(split[2], new CultureInfo("en-US"));

                if (!dates.Contains(date))
                    dates.Add(date);

                if (employeeWorkingTime.TryGetValue(fullName, out var workingTimeList))
                {
                    workingTimeList.Add(new WorkingTime
                    {
                        Date = date,
                        Hours = hours
                    });
                }
                else
                {
                    var workingTime = new WorkingTime
                    {
                        Date = date,
                        Hours = hours
                    };
                    employeeWorkingTime.Add(fullName, new List<WorkingTime> { workingTime });
                }
            }

            var orderedDates = dates.OrderBy(e => e);

            using (var streamWriter = new StreamWriter("output.csv"))
            {
                streamWriter.Write("Name / Date,");

                foreach (var date in orderedDates)
                {
                    streamWriter.Write(date.ToString("yyyy-MM-dd"));

                    if (orderedDates.Last() != date)
                        streamWriter.Write(",");
                    else
                        streamWriter.Write(Environment.NewLine);
                }

                foreach (var fullName in employeeWorkingTime.Keys)
                {
                    var workingTimeList = employeeWorkingTime[fullName];

                    streamWriter.Write(fullName + ",");

                    foreach (var date in orderedDates)
                    {
                        var workingTime = workingTimeList.FirstOrDefault(e => e.Date == date);
                        if (workingTime == null)
                            streamWriter.Write(0);
                        else
                            streamWriter.Write(workingTime.Hours.ToString(new CultureInfo("en-US")));


                        if (orderedDates.Last() != date)
                            streamWriter.Write(",");
                        else
                            streamWriter.Write(Environment.NewLine);
                    }
                }
            }

        }
    }
}

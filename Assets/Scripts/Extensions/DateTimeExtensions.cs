using System;
using System.Collections.Generic;

namespace SerializableDateTime.Extensions
{
    public static class DateTimeExtensions
    {
        public static List<List<int>> GenerateCalendarMatrix(this DateTime dateTime)
        {
            List<List<int>> matrix = new();

            DateTime firstOfMonth = new(dateTime.Year, dateTime.Month, 1);

            // Start from Sunday, adjust if needed (start of week = Sunday)
            int daysBefore = (int)firstOfMonth.DayOfWeek;

            // Go back to the Sunday (or Monday if you want ISO 8601) before the 1st
            DateTime startDate = firstOfMonth.AddDays(-daysBefore);

            for (int week = 0; week < 6; week++)
            {
                List<int> weekRow = new();

                for (int day = 0; day < 7; day++)
                {
                    weekRow.Add(startDate.Day);
                    startDate = startDate.AddDays(1);
                }

                matrix.Add(weekRow);
            }

            return matrix;
        }
    }
}
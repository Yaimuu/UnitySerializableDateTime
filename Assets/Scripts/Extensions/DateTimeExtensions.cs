using System;
using System.Collections.Generic;

namespace SerializedCalendar.Extension
{
    public static class DateTimeExtensions
    {
        public static List<List<int>> GenerateMonthlyCalendarMatrix(this DateTime dateTime)
        {
            List<List<int>> matrix = new();

            DateTime firstOfMonth = new(dateTime.Year, dateTime.Month, 1);

            // Start from Sunday, adjust if needed (start of week = Sunday)
            int daysBefore = (int)firstOfMonth.DayOfWeek;

            // Go back to the Sunday (or Monday if you want ISO 8601) before the 1st
            DateTime startDate = firstOfMonth.AddDays(-daysBefore);

            for (var week = 0; week < 6; week++)
            {
                List<int> weekRow = new();

                for (var day = 0; day < 7; day++)
                {
                    weekRow.Add(startDate.Day);
                    startDate = startDate.AddDays(1);
                }

                matrix.Add(weekRow);
            }

            return matrix;
        }

        public static List<List<int>> GenerateDecadeCalendarMatrix(this DateTime dateTime)
        {
            List<List<int>> matrix = new();
            
            // Step 1: Find the start of the decade (e.g., 2025 → 2019)
            int startYear = (dateTime.Year / 10) * 10 - 1;

            // Step 2: Fill the 3x4 matrix with years
            for (int row = 0; row < 3; row++)
            {
                List<int> rowList = new();
                for (int col = 0; col < 4; col++)
                {
                    int year = startYear + (row * 4) + col;
                    rowList.Add(year);
                }
                matrix.Add(rowList);
            }
            
            return matrix;
        }

        public static List<List<int>> GenerateCenturyCalendarMatrix(this DateTime dateTime)
        {
            List<List<int>> matrix = new();
            
            // Step 1: Find the start of the century (e.g., 2025 → 1990)
            int startDecade = (dateTime.Year / 100) * 100 - 10;
            
            // Step 2: Fill the 3x4 matrix with years
            for (int row = 0; row < 3; row++)
            {
                List<int> rowList = new();
                for (int col = 0; col < 4; col++)
                {
                    int year = startDecade + (row * 40) + col * 10;
                    rowList.Add(year);
                }
                matrix.Add(rowList);
            }
            
            return matrix;
        }
    }
}
using System.Collections.Generic;
using System.Globalization;

namespace SerializedCalendar.Utils
{
    public static class CalendarUtils
    {
        public static List<List<string>> GetMonthsMatrix()
        {
            var monthsMatrix = new List<List<string>>();
            var monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;

            for (int row = 0; row < 3; row++)
            {
                var rowList = new List<string>();
                for (int col = 0; col < 4; col++)
                {
                    int monthIndex = row * 4 + col;
                    rowList.Add(monthNames[monthIndex]);
                }
                monthsMatrix.Add(rowList);
            }

            return monthsMatrix;
        }
    }
}
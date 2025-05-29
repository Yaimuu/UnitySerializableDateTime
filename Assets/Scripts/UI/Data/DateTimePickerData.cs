using System;
using System.Collections.Generic;
using System.Linq;
using SerializedCalendar.Extension;
using SerializedCalendar.Utils;
using UnityEngine;

namespace SerializedCalendar.UI
{
    [Serializable]
    public class DateTimeCellData : ScriptableObject
    {
        public string dateValue;
    }
    
    [Serializable]
    public class DateTimeRowData
    {
        public List<DateTimeCellData> cells;

        public DateTimeRowData(List<int> row)
        {
            cells = row.ConvertAll(cellValue =>
            {
                DateTimeCellData cell = ScriptableObject.CreateInstance<DateTimeCellData>();
                cell.dateValue = cellValue.ToString();
                return cell;
            });
        }
        
        public DateTimeRowData(List<string> row)
        {
            cells = row.ConvertAll(cellValue =>
            {
                DateTimeCellData cell = ScriptableObject.CreateInstance<DateTimeCellData>();
                cell.dateValue = cellValue;
                return cell;
            });
        }
    }
    
    public class DateTimePickerData : ScriptableObject
    {
        public string title = "Date";
        public List<DateTimeRowData> values = new();
        
        public int ColumnCount => values.Count > 0 ? values.First().cells.Count : 0;

        public void UpdateCalendar(DateTime newDate, CalendarScope scope = CalendarScope.Month)
        {
            values.Clear();
            
            switch (scope)
            {
                case CalendarScope.Time:
                    break;

                case CalendarScope.Year:
                    values = CalendarUtils.GetMonthsMatrix().ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:yyyy}";
                    break;

                case CalendarScope.Decade:
                    SetRangeTitle(newDate.GenerateDecadeCalendarMatrix());
                    break;

                case CalendarScope.Century:
                    SetRangeTitle(newDate.GenerateCenturyCalendarMatrix());
                    break;

                case CalendarScope.Month:
                default:
                    values = newDate.GenerateMonthlyCalendarMatrix().ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:MMMM} {newDate:yyyy}";
                    break;
            }

            void SetRangeTitle(List<List<int>> matrix)
            {
                values = matrix.ConvertAll(row => new DateTimeRowData(row));
                var first = values.FirstOrDefault()?.cells.FirstOrDefault()?.dateValue;
                var last = values.LastOrDefault()?.cells.LastOrDefault()?.dateValue;
                title = $"{first}-{last}";
            }
        }
    }
}
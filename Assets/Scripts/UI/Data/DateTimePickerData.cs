using System;
using System.Collections.Generic;
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

        public void UpdateCalendar(DateTime newDate, CalendarScope scope = CalendarScope.Month)
        {
            values.Clear();
            
            switch (scope)
            {
                case CalendarScope.Time:
                    break;
                case CalendarScope.Year:
                    var newValues = CalendarUtils.GetMonthsMatrix();
                    values = newValues.ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:yyyy}";
                    break;
                case CalendarScope.Decade:
                    break;
                case CalendarScope.Century:
                    break;
                case CalendarScope.Month:
                default:
                    var newIntValues = newDate.GenerateMonthlyCalendarMatrix();
                    values = newIntValues.ConvertAll(row => new DateTimeRowData(row));
                    title = $"{newDate:MMMM} {newDate:yyyy}";
                    break;
            }
        }
    }
}